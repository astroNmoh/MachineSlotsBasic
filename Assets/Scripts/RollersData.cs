using System.Collections.Generic;
using static Reels;
public enum Reels : sbyte { Bell, WaterM, Grape, Plum, Orange, Lemon, Berry }
public class RollersData
{
	public List<List<Reels>> rollers;
	public List<(int, int)[]> winningPatterns;
	public Dictionary<Reels, Dictionary<int, int>> winRewards; //Could be a 2D array for critical performance searches
  public RollersData()
  {
		//This could be handled as Json or any preferred input data
		rollers = new List<List<Reels>>
		{
			new List <Reels> { Orange, Bell, WaterM, Berry, Plum, Lemon, Grape, Plum, Bell, Bell, Orange, Grape, Lemon, Lemon},
			new List <Reels> { WaterM, Berry, Bell, Plum, Berry, Grape, Orange, Lemon, Lemon, Lemon, Berry, Lemon, Plum, Lemon, Berry},
			new List <Reels> { Grape, WaterM, Plum, Grape, Bell, Lemon, Berry, Bell, Lemon, Lemon, Orange, Orange, Grape},
			new List <Reels> { Lemon, Plum, Plum, Lemon, Grape, Orange, WaterM, WaterM, Bell, Berry, Berry, Lemon, Orange, Plum, Lemon},
			new List <Reels> { Grape, Berry, Bell, WaterM, Orange, Orange, Lemon, Plum, Orange, Lemon, Grape, Bell, WaterM, Berry}
		};

		winningPatterns = new List<(int, int)[]>()
		{
			//We could add patterns dinamically, or same sent as json
			new (int, int)[] { (0, 2), (1, 0), (2, 2), (3, 0), (4, 2) },
			new (int, int)[] { (0, 0), (1, 2), (2, 0), (3, 2), (4, 0) },
			new (int, int)[] { (0, 0), (1, 1), (2, 2), (3, 1), (4, 0) }
		};

		winRewards = new Dictionary<Reels, Dictionary<int, int>>()
		{
			{ Bell, new Dictionary<int, int>
				{
					{ 2, 25 },
					{ 3, 50 },
					{ 4, 75 },
					{ 5, 100 }
				}
			},
			{ Plum, new Dictionary<int, int>
				{
					{ 2, 5 },
					{ 3, 10 },
					{ 4, 20 },
					{ 5, 40 }
				}
			},
			{ Berry, new Dictionary<int, int>
				{
					{ 2, 1 },
					{ 3, 2 },
					{ 4, 5 },
					{ 5, 10 }
				}
			},
			{ WaterM, new Dictionary<int, int>
				{
					{ 2, 10 },
					{ 3, 20 },
					{ 4, 30 },
					{ 5, 60 }
				}
			},
			{ Orange, new Dictionary<int, int>
				{
					{ 2, 5 },
					{ 3, 10 },
					{ 4, 15 },
					{ 5, 30 }
				}
			},
			{ Grape, new Dictionary<int, int>
				{
					{ 2, 5 },
					{ 3, 10 },
					{ 4, 20 },
					{ 5, 50 }
				}
			},
			{ Lemon, new Dictionary<int, int>
				{
					{ 2, 2 },
					{ 3, 5 },
					{ 4, 10 },
					{ 5, 20 }
				}
			}
		};
	}
}