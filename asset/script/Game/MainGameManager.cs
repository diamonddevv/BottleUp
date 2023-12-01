using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static BottleUp.asset.script.Game.DeliverableItems;
using static BottleUp.asset.script.Util.BottleUpHelper;

public partial class MainGameManager : Node
{
    [Signal]
    public delegate void ActiveRequestsUpdatedEventHandler();

    [Signal]
    public delegate void DeliveryMadeEventHandler();

	[ExportCategory("Components")]
	[Export] public Timer GameTimer;
	[Export] public Map Map;
	[Export] public Player Player;

	[ExportCategory("Settings")]
	[Export] public double GameSeconds;
    [Export] public int MaxRequests = 10;

    [ExportCategory("Miscellaneous")]
    [Export] public PackedScene FinishScreen;


    private Random random;
	private bool _started = false;
    private Dictionary<Poi, DeliveryRequest> _activeRequests = new Dictionary<Poi, DeliveryRequest>();

    public void MarkRequestUpdate() => EmitSignal(SignalName.ActiveRequestsUpdated);

    public override void _Ready()
    {
        random = new Random();

        Map.DeliveryMade += (poi) =>
        {
            if (_activeRequests.ContainsKey(poi))
            {
                var v = _activeRequests[poi];
                v.SetRatings(GameTimer.TimeLeft);
                Player.GetCompletedDeliveries().Add(v);
                EmitSignal(SignalName.DeliveryMade);
                poi.SetDelivery(null);
            }
        };

        Map.GameManager = this;

		Start();
	}

	
	public override void _Process(double delta)
	{
		if (_started)
		{
            // Add Requests
            if (_activeRequests.Count < MaxRequests)
            {
                if (random.Chance(1f/50f * (GameSeconds - GameTimer.TimeLeft < 60 ? 100f : 1f)))
                {
                    // Add a Request
                    var dest = random.RandomElement(Map.GetDestinations().FindAll((poi) => !_activeRequests.ContainsKey(poi))); // gives a random poi without a request already present

                    DeliveryRequest req = DeliveryRequest.WithRandomFields(random, GameTimer, 3);

                    _activeRequests.Add(dest, req);
                    MarkRequestUpdate();
                    dest.SetDelivery(req);
                }
            }
		}
	}


	public void Start()
	{
		_started = true;
		GameTimer.Start(GameSeconds);
		GameTimer.Timeout += FinishGame;
	}

    private void FinishGame()
    {
		_started = false;

        double avgRating = 0;
        double avgSpeedRating = 0;
        double avgIntactness = 0;

        foreach (var v in Player.GetCompletedDeliveries())
        {
            avgSpeedRating += v.Speed.Percentage;
            avgIntactness += v.MilkIntactness.Percentage;

            avgRating += v.Average.Percentage;
        }


        avgRating /= Player.GetCompletedDeliveries().Count;
        avgIntactness /= Player.GetCompletedDeliveries().Count;
        avgSpeedRating /= Player.GetCompletedDeliveries().Count;

        Player.SetRating(new Rating()
        {
            StarCount = 5,
            Percentage = avgRating
        });

        // change to finish screen
        Finish finish = FinishScreen.Instantiate<Finish>();
        finish.StarsPercentage = Player.GetRating().Percentage * 100 * Player.GetCompletedDeliveries().Count / 8;

        if (double.IsNaN(finish.StarsPercentage)) finish.StarsPercentage = 0;

        "\n".Test();
        $"Avg. Intactness: {avgIntactness * 100}".Test();
        $"Avg. Speed Rating: {avgSpeedRating * 100}".Test();
        $"Score: {finish.StarsPercentage}".Test();

        GetTree().Root.GetChild(0).QueueFree();
        GetTree().Root.AddChild(finish);
    }

    public struct DeliveryRequest
    {
        public List<CountedItem> Items;
        public PrioritySpeed Priority;
        public double DispatchTime;

        public bool Made;

        public Rating Average;
        public Rating MilkIntactness;
        public Rating Speed;

        public void SetRatings(double deliveryTime)
        {
            double speed = DispatchTime - deliveryTime;
            float averageItemIntactness = 0;
            float totalCount = 0;

            foreach (var item in Items)
            {
                averageItemIntactness += item.intactness;
                totalCount += item.count;
            }

            averageItemIntactness /= Items.Count;


            Speed = new Rating()
            {
                StarCount = 5,
                Percentage = Mathf.Max(0, (Priority.Time - speed) / Priority.Time)
            };

            MilkIntactness = new Rating()
            {
                StarCount = 5,
                Percentage =  averageItemIntactness
            };

            CalculateAverage();
        }
        public void CalculateAverage()
        {
            double a = 0;

            a += MilkIntactness.Percentage;
            a += Speed.Percentage;

            a /= 2;

            Average = new Rating()
            {
                StarCount = 5,
                Percentage = a
            };
        }


        public static DeliveryRequest WithRandomFields(Random random, Timer timer, int maxItems = 1)
        {
            var req = new DeliveryRequest();

            int items = maxItems > 1 ? random.Next(maxItems) + 1 : 1;
            int currentWeight = 0;
            req.Items = new List<CountedItem>();

            List<EnumItem> alreadyPresentItems = new List<EnumItem>();
            for (int i = 0; i < items; i++)
            {
                var item = new CountedItem();
                item.item = RandomItem(random, alreadyPresentItems);

                item.count = random.Next(GetByEnum(item.item).MaxDeliverable) + 1;
                item.intactness = 1;

                if (!(currentWeight + (item.count * GetByEnum(item.item).MilkUnitWeight) > PlayerInventoryHandler.MaxMilkUnitsCarriable))
                {
                    currentWeight += item.count * GetByEnum(item.item).MilkUnitWeight;
                    alreadyPresentItems.Add(item.item); 
                    req.Items.Add(item);

                    if (item.item == EnumItem.Cow)
                    {
                        break;
                    }
                } else
                {
                    if (i < 2) i -= 1;
                }
            }

            PrioritySpeed.Init();
            req.Priority = random.RandomElement(PrioritySpeed.PRIORITY_SPEEDS, (p) => p.Weight);
            req.DispatchTime = timer.TimeLeft;
            req.Made = false;

            return req;
        }

        public void SetIntactness(PlayerInventoryHandler inventoryHandler)
        {
            foreach (var item in inventoryHandler.GetCarriedItemsWithCount())
            {
                if (!Items.Any(i => i.item == item.Key.item)) continue;

                CountedItem i = Items.First(e => e.item == item.Key.item);
                int index = Items.IndexOf(i);
                var temp = Items[index];
                temp.intactness = item.Key.damage;
                Items[index] = temp;
            }
            
        }

        public override string ToString()
        {
            return $"[{string.Join(",", Items)} - {Speed.Percentage},{MilkIntactness.Percentage},{Average.Percentage}]";
        }
    }

    public Dictionary<Poi, DeliveryRequest> GetActiveRequests() => _activeRequests;

    public struct CountedItem
    {
        public EnumItem item;
        public int count;
        public float intactness;

        public override string ToString()
        {
            return $"[{item},{count},{intactness}]";
        }
    }
    public struct PrioritySpeed
    {
        public static List<PrioritySpeed> PRIORITY_SPEEDS = new List<PrioritySpeed>();

        public static void Init()
        {
            PRIORITY_SPEEDS.Add(LOW);
            PRIORITY_SPEEDS.Add(MID);
            PRIORITY_SPEEDS.Add(HIGH);
        }

        public static PrioritySpeed LOW = new() { Time = 120, Name = "Low", Color = 0x3bba00ff, Weight = 10 };
        public static PrioritySpeed MID = new() { Time = 60, Name = "Medium", Color = 0xfcf33dff, Weight = 5 };
        public static PrioritySpeed HIGH = new() { Time = 30, Name = "High", Color = 0xcf0404ff, Weight = 2 };

        public double Time;
        public string Name;
        public uint Weight;
        public uint Color;

        public override string ToString()
        {
            return $"[{Name},{Color},{Time},{Weight}]";
        }
    }
}
