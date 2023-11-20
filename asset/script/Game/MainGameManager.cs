using Godot;
using System;
using System.Collections.Generic;
using static BottleUp.asset.script.Game.DeliverableItems;
using static BottleUp.asset.script.Util.BottleUpHelper;

public partial class MainGameManager : Node
{
	[ExportCategory("Components")]
	[Export] public Timer GameTimer;
	[Export] public Map Map;

	[ExportCategory("Settings")]
	[Export] public double GameSeconds;

	private bool _started = false;
    private List<DeliveryRequest> _completedDeliveries;
	private Rating _rating;


    public override void _Ready()
    {

        Map.DeliveryMade += (poi) =>
        {
            var v = poi.GetDelivery();
            _completedDeliveries.Add(v);
        };

		Start();
	}

	
	public override void _Process(double delta)
	{
		if (_started)
		{

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

        float avgRating = 0;

        foreach (var v in _completedDeliveries) avgRating += v.Average.Percentage;
        avgRating /= _completedDeliveries.Count;

        _rating = new Rating()
        {
            StarCount = 5,
            Percentage = avgRating
        };

        // change to finish screen
    }

    public struct DeliveryRequest
    {
        public CountedItem[] Items;
        public PrioritySpeed Priority;
        public float DispatchTime;

        public bool Made;

        public Rating Average;
        public Rating MilkIntactness;
        public Rating Quantity;
        public Rating Speed;

        public void SetRatings(float deliveryTime, CountedItem[] delivered)
        {
            float speed = deliveryTime - DispatchTime;
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

            averageItemIntactness /= delivered.Length;


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
            float a = 0;

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
    }

    public struct CountedItem
    {
        public EnumItem item;
        public int count;
        public float intactness;
    }

    public struct PrioritySpeed
    {
        public static readonly PrioritySpeed LOW = new() { Time = 120, Name = "Low Priority", Color = 0x3bba00 };
        public static readonly PrioritySpeed MID = new() { Time = 60, Name = "Medium Priority", Color = 0xfcf33d };
        public static readonly PrioritySpeed HIGH = new() { Time = 30, Name = "High Priority", Color = 0xcf0404 };

        public float Time;
        public string Name;
        public int Color;
    }
}
