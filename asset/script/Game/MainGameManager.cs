using BottleUp.asset.script.Game;
using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Xml;
using static BottleUp.asset.script.Game.DeliverableItems;
using static BottleUp.asset.script.Util.BottleUpHelper;

public partial class MainGameManager : Node
{
    [Signal]
    public delegate void ActiveRequestsUpdatedEventHandler();


	[ExportCategory("Components")]
	[Export] public Timer GameTimer;
	[Export] public Map Map;
	[Export] public Player Player;

	[ExportCategory("Settings")]
	[Export] public double GameSeconds;
    [Export] public int MaxRequests = 8;


    private Random random;
	private bool _started = false;
    private Dictionary<Poi, DeliveryRequest> _activeRequests = new Dictionary<Poi, DeliveryRequest>();

    public void MarkRequestUpdate() => EmitSignal(SignalName.ActiveRequestsUpdated);

    public override void _Ready()
    {
        random = new Random();

        Map.DeliveryMade += (poi) =>
        {
            if (poi.GetDelivery() != null)
            {
                var v = poi.GetDelivery();
                Player.GetCompletedDeliveries().Add(v.Value);
            }
        };

		Start();
	}

	
	public override void _Process(double delta)
	{
		if (_started)
		{
            // Add Requests
            if (_activeRequests.Count < MaxRequests)
            {
                if (random.Chance(1f/50f))
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

        foreach (var v in Player.GetCompletedDeliveries()) avgRating += v.Average.Percentage;
        avgRating /= Player.GetCompletedDeliveries().Count;

        Player.SetRating(new Rating()
        {
            StarCount = 5,
            Percentage = avgRating
        });

        // change to finish screen
    }

    public struct DeliveryRequest
    {
        public List<CountedItem> Items;
        public PrioritySpeed Priority;
        public double DispatchTime;

        public bool Made;

        public Rating Average;
        public Rating MilkIntactness;
        public Rating Quantity;
        public Rating Speed;

        public void SetRatings(float deliveryTime, List<CountedItem> delivered)
        {
            double speed = deliveryTime - DispatchTime;
            float averageItemIntactness = 0;
            float desiredCount = 0;
            float totalCount = 0;

            foreach (var item in delivered)
            {
                averageItemIntactness += item.intactness;
                totalCount += item.count;
            }

            foreach (var item in Items)
            {
                desiredCount += item.count;
            }

            averageItemIntactness /= delivered.Count;


            Speed = new Rating()
            {
                StarCount = 5,
                Percentage = Priority.Time / speed
            };

            MilkIntactness = new Rating()
            {
                StarCount = 5,
                Percentage = averageItemIntactness
            };

            Quantity = new Rating()
            {
                StarCount = 5,
                Percentage = totalCount / desiredCount
            };

            CalculateAverage();
        }
        public void CalculateAverage()
        {
            double a = 0;

            a += MilkIntactness.Percentage;
            a += Quantity.Percentage;
            a += Speed.Percentage;

            a /= 3;

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


            for (int i = 0; i < items; i++)
            {
                var item = new CountedItem();
                item.item = RandomItem(random);
                item.count = random.Next(GetByEnum(item.item).MaxDeliverable) + 1;
                item.intactness = 1;

                if (!(currentWeight + (item.count * GetByEnum(item.item).MilkUnitWeight) > PlayerInventoryHandler.MaxMilkUnitsCarriable))
                {
                    currentWeight += item.count * GetByEnum(item.item).MilkUnitWeight;
                    req.Items.Add(item);
                } else
                {
                    if (i < 2) i -= 1;
                }
            }

            req.Priority = random.RandomElement(PrioritySpeed.PRIORITY_SPEEDS);
            req.DispatchTime = timer.TimeLeft;
            req.Made = false;

            return req;
        }
    }

    public Dictionary<Poi, DeliveryRequest> GetActiveRequests() => _activeRequests;

    public struct CountedItem
    {
        public EnumItem item;
        public int count;
        public float intactness;
    }
    public struct PrioritySpeed
    {
        public static readonly List<PrioritySpeed> PRIORITY_SPEEDS = new List<PrioritySpeed>() { LOW, MID, HIGH };

        public static readonly PrioritySpeed LOW = new() { Time = 120, Name = "Low Priority", Color = 0x3bba00 };
        public static readonly PrioritySpeed MID = new() { Time = 60, Name = "Medium Priority", Color = 0xfcf33d };
        public static readonly PrioritySpeed HIGH = new() { Time = 30, Name = "High Priority", Color = 0xcf0404 };

        public double Time;
        public string Name;
        public int Color;
    }
}
