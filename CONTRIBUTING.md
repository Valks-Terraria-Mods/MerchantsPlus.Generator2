# Contributing

## What is MerchantsPlus?
If you are unfamiliar with the MerchantsPlus mod you can read up on it here  
https://steamcommunity.com/sharedfiles/filedetails/?id=2817831855  

Basically every town NPC (including a few other NPCs like the old man) are merchants now. Terraria shops are limited to 40 items so that's why shots are split into groups of 40 but now I'm realizing that I could just use a custom UI in-game to overcome this limitation. Not really sure what to do.

## Running the project
Make sure your .NET is at least `8.0`. Run the project with `F5` from within VSCode or use `dotnet run`.

## What needs to be done?
This generator is currently not being used by the mod because it's not up to my standards yet.

The majority of items are not ordered by progression. For example the easiest wood type to obtain is `Wooden` and then the next easiest to get would be maybe `Cactus` and so on. We need to do this for every item. This is a not a easy task.

Shops can have shops with in them. For example the merchant "Merchant" could have a shop called "Gear" and one item slot could occupy a 'shop' but it is not really a 'shop' it is just a item that gets better over time based on the current met item conditions. For example say the 'shop' item is for pickaxes, the first pickaxe would be a copper pickaxe but then when you say beat the Slime King then that copper pickaxe could be upgraded to a silver pickaxe and so on.

The shops need to be evenly distributed among the merchants. But there are some special cases. For example I think the steampunker should have all furniture related shops. Earlier I mentioned a 40 item cap per shop but I might remove this in the actual mod later. I just don't know right now. 

## Further plans
For items with unlock conditions they should have associated hint texts on how to unlock a certain condition. I'm still deciding whether this should be defined in the generator or in the mod or a little bit of both.

## Note
I'm not sure if it's the best way to read in the assembly like the way I'm doing it now or maybe I should have just added the assembly as a project reference and maybe that would have been easier. I'm not sure. I want to have more item conditions to work with later down the line.

## Contact
You can talk to me over discord, my username is `valky5`.
