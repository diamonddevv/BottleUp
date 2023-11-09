using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottleUp.asset.script.Game
{
    internal class DeliverableItems
    {
        public const uint
            COW = 0,
            STRAWBERRY = 0,
            BANANA = 0,
            CHOCO = 0,
            COCO = 0,
            OAT = 0,
            ALMOND = 0,
            SOYA = 0,


        public struct Item
        {
            public string Name;
            public string Description;

            public bool IsMilkBottle;
            public uint TextureIndex;

            public int MilkUnitWeight;
        }

        public static List<Item> Items = new List<Item>()
        {
            new Item()
            {
                Name = "Cow Milk",
                Description = "Your standard, run-of-the-mill Cow's milk.",

                IsMilkBottle = true,
                TextureIndex = 0,

                MilkUnitWeight = 1,
            },
            new Item()
            {
                Name = "Strawberry Milk",
                Description = "Cow's milk, but fruitier!",

                IsMilkBottle = true,
                TextureIndex = 0,

                MilkUnitWeight = 2,
            },
            new Item()
            {
                Name = "Banana Milk",
                Description = "Fit for a minion!",

                IsMilkBottle = true,
                TextureIndex = 0,

                MilkUnitWeight = 2,
            },
            new Item()
            {
                Name = "Chocolate Milk",
                Description = "CHOCCY MILK!!",

                IsMilkBottle = true,
                TextureIndex = 0,

                MilkUnitWeight = 2,
            },
            new Item()
            {
                Name = "Coconut Milk",
                Description = "The coconut nut is a giant nut, but this delicious nut is not a nut.",

                IsMilkBottle = true,
                TextureIndex = 0,

                MilkUnitWeight = 3,
            },
            new Item()
            {
                Name = "Oat Milk",
                Description = "Imagine being lactose intolerant.",

                IsMilkBottle = true,
                TextureIndex = 0,

                MilkUnitWeight = 3,
            },
            new Item()
            {
                Name = "Almond Milk",
                Description = "How do you milk an almond..?",

                IsMilkBottle = true,
                TextureIndex = 0,

                MilkUnitWeight = 3,
            },
            new Item()
            {
                Name = "Soya Milk",
                Description = "Milk minus Cow.",

                IsMilkBottle = true,
                TextureIndex = 0,

                MilkUnitWeight = 3,
            },
            new Item()
            {
                Name = "Cheese",
                Description = "Milk, but we did some chemistry with it.",

                IsMilkBottle = false,
                TextureIndex = 0,

                MilkUnitWeight = 5,
            },
            new Item()
            {
                Name = "Cream",
                Description = "Milk with milk in a different form mixed together.",

                IsMilkBottle = false,
                TextureIndex = 0,

                MilkUnitWeight = 5,
            },
            new Item()
            {
                Name = "Cow",
                Description = "\"What do you mean you want the ENTIRE cow??\"",

                IsMilkBottle = false,
                TextureIndex = 0,

                MilkUnitWeight = 5,
            },
        };
    }
}
