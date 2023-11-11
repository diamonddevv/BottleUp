using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottleUp.asset.script.Game
{
    public class DeliverableItems
    {
        public enum EnumItem
        {
            CowMilk = 0,
            StrawberryMilk = 1,
            BananaMilk = 2,
            ChocolateMilk = 3,
            CoconutMilk = 4,
            OatMilk = 5,
            AlmondMilk = 6,
            SoyaMilk = 7,
            Cheese = 8,
            Cream = 9,
            Butter = 10,
            Cow = 11
        }

        public static Item GetByEnum(EnumItem item) => Items[(int)item];

        public struct Item
        {
            public string Name;
            public string Description;

            public bool IsMilkBottle;
            public Vector4 MilkColor;
            public int TextureIndex;

            public int MilkUnitWeight;
        }

        public static List<Item> Items = new List<Item>()
        {
            new Item()
            {
                Name = "Cow Milk",
                Description = "Your standard, run-of-the-mill Cow's milk.",

                IsMilkBottle = true,
                MilkColor = new Vector4(.8f,.8f,.8f,1),
                TextureIndex = 0,

                MilkUnitWeight = 1,
            },
            new Item()
            {
                Name = "Strawberry Milk",
                Description = "Cow's milk, but fruitier!",

                IsMilkBottle = true,
                MilkColor = new Vector4(1f,.75f,.78f,1),
                TextureIndex = 1,

                MilkUnitWeight = 2,
            },
            new Item()
            {
                Name = "Banana Milk",
                Description = "Fit for a minion!",

                IsMilkBottle = true,
                MilkColor = new Vector4(.89f,.83f,.58f,1),
                TextureIndex = 2,

                MilkUnitWeight = 2,
            },
            new Item()
            {
                Name = "Chocolate Milk",
                Description = "CHOCCY MILK!!",

                IsMilkBottle = true,
                MilkColor = new Vector4(.32f,.25f,.21f,1),
                TextureIndex = 3,

                MilkUnitWeight = 2,
            },
            new Item()
            {
                Name = "Coconut Milk",
                Description = "The coconut nut is a giant nut, but this delicious nut is not a nut.",

                IsMilkBottle = true,
                MilkColor = new Vector4(.9f,.9f,.9f,1),
                TextureIndex = 4,

                MilkUnitWeight = 3,
            },
            new Item()
            {
                Name = "Oat Milk",
                Description = "Imagine being lactose intolerant.",

                IsMilkBottle = true,
                MilkColor = new Vector4(.95f,.95f,.95f,1),
                TextureIndex = 5,

                MilkUnitWeight = 3,
            },
            new Item()
            {
                Name = "Almond Milk",
                Description = "How do you milk an almond..?",

                IsMilkBottle = true,
                MilkColor = new Vector4(.98f,.89f,.82f,1),
                TextureIndex = 6,

                MilkUnitWeight = 3,
            },
            new Item()
            {
                Name = "Soya Milk",
                Description = "Milk minus Cow.",

                IsMilkBottle = true,
                MilkColor = new Vector4(1,1,1,1),
                TextureIndex = 7,

                MilkUnitWeight = 3,
            },
            new Item()
            {
                Name = "Cheese",
                Description = "Ye olde bacterial milk.",

                IsMilkBottle = false,
                TextureIndex = 8,

                MilkUnitWeight = 5,
            },
            new Item()
            {
                Name = "Cream",
                Description = "Milk minus.. most of the milk. It's part of it, I suppose.",

                IsMilkBottle = false,
                TextureIndex = 9,

                MilkUnitWeight = 5,
            },
            new Item()
            {
                Name = "Butter",
                Description = "Milk in solid form. Pretty much.",

                IsMilkBottle = false,
                TextureIndex = 10,

                MilkUnitWeight = 5,
            },
            new Item()
            {
                Name = "Cow",
                Description = "\"What do you mean you want the ENTIRE cow??\"",

                IsMilkBottle = false,
                TextureIndex = 11,

                MilkUnitWeight = 20,
            },
        };
    }
}
