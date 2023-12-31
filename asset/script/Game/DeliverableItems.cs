﻿using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottleUp.asset.script.Game
{
    public static class DeliverableItems
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
        public static EnumItem RandomItem(Random random) => (EnumItem)random.Next(12);
        public static EnumItem RandomItem(Random random, List<EnumItem> not)
        {
            EnumItem? item = null;

            while (!item.HasValue)
            {
                item = RandomItem(random);
                if (not.Contains(item.Value))
                {
                    item = null;
                }
            }

            return item.Value;
        }

        public struct Item
        {
            public string Name;
            public string Description;

            public int MaxDeliverable;

            public bool IsMilkBottle;
            public Vector4 MilkColor;
            public int TextureIndex;

            public int MilkUnitWeight;

            public EnumItem Enum;
        }

        public static List<Item> Items = new List<Item>()
        {
            new Item()
            {
                Name = "Cow Milk",
                Description = "Your standard, run-of-the-mill Cow's milk.",

                MaxDeliverable = 15,

                IsMilkBottle = true,
                MilkColor = new Vector4(.9f,.9f,.9f,1),
                TextureIndex = 0,

                MilkUnitWeight = 1,

                Enum = EnumItem.CowMilk,
            },
            new Item()
            {
                Name = "Strawberry Milk",
                Description = "Cow's milk, but fruitier!",

                MaxDeliverable = 8,

                IsMilkBottle = true,
                MilkColor = new Vector4(1f,.75f,.78f,1),
                TextureIndex = 1,

                MilkUnitWeight = 2,

                Enum = EnumItem.StrawberryMilk,
            },
            new Item()
            {
                Name = "Banana Milk",
                Description = "\"10/10\" - Monkeys across the globe",

                MaxDeliverable = 8,

                IsMilkBottle = true,
                MilkColor = new Vector4(.89f,.83f,.58f,1),
                TextureIndex = 2,

                MilkUnitWeight = 2,

                Enum = EnumItem.BananaMilk,
            },
            new Item()
            {
                Name = "Chocolate Milk",
                Description = "CHOCCY MILK!!",

                MaxDeliverable = 8,

                IsMilkBottle = true,
                MilkColor = new Vector4(.32f,.25f,.21f,1),
                TextureIndex = 3,

                MilkUnitWeight = 2,

                Enum = EnumItem.ChocolateMilk,
            },
            new Item()
            {
                Name = "Coconut Milk",
                Description = "The coconut nut is a giant nut, \nbut this delicious nut is not a nut.",

                MaxDeliverable = 5,

                IsMilkBottle = true,
                MilkColor = new Vector4(.9f,.9f,.9f,1),
                TextureIndex = 4,

                MilkUnitWeight = 3,

                Enum = EnumItem.CoconutMilk,
            },
            new Item()
            {
                Name = "Oat Milk",
                Description = "Imagine being lactose intolerant.",

                MaxDeliverable = 5,

                IsMilkBottle = true,
                MilkColor = new Vector4(.95f,.95f,.95f,1),
                TextureIndex = 5,

                MilkUnitWeight = 3,

                Enum = EnumItem.OatMilk,
            },
            new Item()
            {
                Name = "Almond Milk",
                Description = "How do you milk an almond..?",

                MaxDeliverable = 5,

                IsMilkBottle = true,
                MilkColor = new Vector4(.98f,.89f,.82f,1),
                TextureIndex = 6,

                MilkUnitWeight = 3,

                Enum = EnumItem.AlmondMilk,
            },
            new Item()
            {
                Name = "Soya Milk",
                Description = "Milk minus Cow.",

                MaxDeliverable = 5,

                IsMilkBottle = true,
                MilkColor = new Vector4(1,1,1,1),
                TextureIndex = 7,

                MilkUnitWeight = 3,

                Enum = EnumItem.SoyaMilk,
            },
            new Item()
            {
                Name = "Cheese",
                Description = "Ye olde bacterial milk.",

                MaxDeliverable = 3,

                IsMilkBottle = false,
                TextureIndex = 8,

                MilkUnitWeight = 5,

                Enum = EnumItem.Cheese,
            },
            new Item()
            {
                Name = "Cream",
                Description = "Milk minus.. most of the milk. \nIt's part of it, I suppose.",

                MaxDeliverable = 3,

                IsMilkBottle = false,
                TextureIndex = 9,

                MilkUnitWeight = 5,

                Enum = EnumItem.Cream,
            },
            new Item()
            {
                Name = "Butter",
                Description = "Milk in solid form. Pretty much.",

                MaxDeliverable = 3,

                IsMilkBottle = false,
                TextureIndex = 10,

                MilkUnitWeight = 5,

                Enum = EnumItem.Butter,
            },
            new Item()
            {
                Name = "Cow",
                Description = "\"What do you mean you want the ENTIRE cow??\"",

                MaxDeliverable = 1,

                IsMilkBottle = false,
                TextureIndex = 11,

                MilkUnitWeight = 20,

                Enum = EnumItem.Cow,
            },
        };
    }
}
