using System.Linq.Expressions;
using System.Reflection;

namespace MerchantsPlus.Generator;

internal static class ItemPriceMetadataLoader
{
    internal static void LoadTypeAndIdMetadata(
        Dictionary<string, int> itemIdByName,
        out Type? itemType,
        out Func<object>? itemFactory,
        out Action<object, int, bool>? setDefaultsSafeAction,
        out Action<object, int>? setDefaultsAction,
        out Func<object, int>? valueGetter)
    {
        var tmodloaderDllPath = ItemPriceReflectionEnvironment.GetTModLoaderDllPath()
            ?? throw new InvalidOperationException("Unable to find tModLoader.dll");

        var dllLookup = ItemPriceReflectionEnvironment.BuildDllLookup(tmodloaderDllPath);
        ItemPriceReflectionEnvironment.WireAssemblyResolver(dllLookup);

        var assembly = Assembly.LoadFrom(tmodloaderDllPath);
        ItemPriceReflectionEnvironment.InitializeProgramPaths(assembly);

        var itemIdType = assembly.GetType("Terraria.ID.ItemID")
            ?? throw new InvalidOperationException("Unable to find Terraria.ID.ItemID type");

        itemType = assembly.GetType("Terraria.Item")
            ?? throw new InvalidOperationException("Unable to find Terraria.Item type");

        var setDefaultsSafe = itemType.GetMethod("SetDefaults", [typeof(int), typeof(bool)]);
        var setDefaults = itemType.GetMethod("SetDefaults", [typeof(int)])
            ?? throw new InvalidOperationException("Unable to find Terraria.Item.SetDefaults method");

        var valueField = itemType.GetField("value", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException("Unable to find Terraria.Item.value field");

        itemFactory = BuildItemFactory(itemType);
        setDefaultsSafeAction = setDefaultsSafe is null ? null : BuildSetDefaultsSafeAction(itemType, setDefaultsSafe);
        setDefaultsAction = BuildSetDefaultsAction(itemType, setDefaults);
        valueGetter = BuildValueGetter(itemType, valueField);

        var itemFields = itemIdType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly && (f.FieldType == typeof(int) || f.FieldType == typeof(short)));

        var itemCount = itemFields
            .Where(f => string.Equals(f.Name, "Count", StringComparison.OrdinalIgnoreCase))
            .Select(f => Convert.ToInt32(f.GetRawConstantValue()))
            .FirstOrDefault();

        foreach (var field in itemFields)
        {
            var id = Convert.ToInt32(field.GetRawConstantValue());

            if (id <= 0)
                continue;

            if (string.Equals(field.Name, "Count", StringComparison.OrdinalIgnoreCase))
                continue;

            if (itemCount > 0 && id >= itemCount)
                continue;

            itemIdByName[field.Name] = id;
        }
    }

    internal static int ResolveDefaultPrice(
        int itemId,
        IReadOnlySet<int> knownSetDefaultsNullRefIds,
        HashSet<int> failedDefaultPriceIds,
        Type? itemType,
        Func<object>? itemFactory,
        Action<object, int, bool>? setDefaultsSafeAction,
        Action<object, int>? setDefaultsAction,
        Func<object, int>? valueGetter)
    {
        if (itemId <= 0)
            return 0;

        if (knownSetDefaultsNullRefIds.Contains(itemId) || failedDefaultPriceIds.Contains(itemId))
            return 0;

        if (itemType is null || setDefaultsAction is null || valueGetter is null)
            return 0;

        try
        {
            var item = itemFactory?.Invoke() ?? Activator.CreateInstance(itemType);
            if (item is null)
                return 0;

            if (setDefaultsSafeAction is not null)
                setDefaultsSafeAction(item, itemId, true);
            else
                setDefaultsAction(item, itemId);

            return Math.Max(0, valueGetter(item));
        }
        catch
        {
            failedDefaultPriceIds.Add(itemId);
            return 0;
        }
    }

    private static Func<object> BuildItemFactory(Type itemType)
    {
        var ctor = itemType.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException("Unable to find Terraria.Item default constructor");

        var body = Expression.Convert(Expression.New(ctor), typeof(object));
        return Expression.Lambda<Func<object>>(body).Compile();
    }

    private static Action<object, int, bool> BuildSetDefaultsSafeAction(Type itemType, MethodInfo method)
    {
        var itemParam = Expression.Parameter(typeof(object), "item");
        var idParam = Expression.Parameter(typeof(int), "id");
        var noMatCheckParam = Expression.Parameter(typeof(bool), "noMatCheck");

        var call = Expression.Call(Expression.Convert(itemParam, itemType), method, idParam, noMatCheckParam);
        return Expression.Lambda<Action<object, int, bool>>(call, itemParam, idParam, noMatCheckParam).Compile();
    }

    private static Action<object, int> BuildSetDefaultsAction(Type itemType, MethodInfo method)
    {
        var itemParam = Expression.Parameter(typeof(object), "item");
        var idParam = Expression.Parameter(typeof(int), "id");

        var call = Expression.Call(Expression.Convert(itemParam, itemType), method, idParam);
        return Expression.Lambda<Action<object, int>>(call, itemParam, idParam).Compile();
    }

    private static Func<object, int> BuildValueGetter(Type itemType, FieldInfo valueField)
    {
        var itemParam = Expression.Parameter(typeof(object), "item");
        var fieldAccess = Expression.Field(Expression.Convert(itemParam, itemType), valueField);
        var castToInt = Expression.Convert(fieldAccess, typeof(int));
        return Expression.Lambda<Func<object, int>>(castToInt, itemParam).Compile();
    }
}
