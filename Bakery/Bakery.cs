using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BakeryProject
{
    public class Bakery
    {
        public List<Pastry> Stock;

        public Bakery(string inventoryFile)
        {
            Stock = LoadPastries(inventoryFile);
        }

        public List<Pastry> LoadPastries(string inventoryFile)
        {
            using (StreamReader r = new StreamReader(inventoryFile))
            {
                var json = r.ReadToEnd();
                var pastries = JsonConvert.DeserializeObject<List<Pastry>>(json);
                return pastries;
            }
        }

        // reads an order from json
        public List<OrderItem> LoadOrder(string orderName)
        {
            using (StreamReader r = new StreamReader($"../../../Orders/{orderName}"))
            {
                var json = r.ReadToEnd();
                var order = JsonConvert.DeserializeObject<List<OrderItem>>(json);
                return order;
            }
        }

        // calculates the order cost
        public void CalculateCost(OrderItem orderItem)
        {
            var pastryType = Stock.First(i => i.Code == orderItem.Code);
            var possiblePackSize = pastryType.Packs.Select(p => p.PackSize).ToArray();

            // Find possible combinations of pack sizes to satisfy the order
            var possibleBreakDowns = FindCombos(possiblePackSize, orderItem.Quantity, new List<LineItem>(), 0);

            if (possibleBreakDowns.Count > 0)
            {
                // Sort so that the combination that uses the fewest boxes and has the highest price is first
                var result = possibleBreakDowns.OrderBy(
                    combination => combination.Sum(item => item.PackCount)
                ).ThenByDescending(
                    combination => combination.Sum(item => (item.PackCount * (pastryType.Packs.First(pack => pack.PackSize == item.PackSize)).Price))
                );

                var packBreakDown = result.First();
                var totalPrice = packBreakDown.Sum(item => (item.PackCount * (pastryType.Packs.First(pack => pack.PackSize == item.PackSize)).Price));

                //Output the resulting pack breakdown and price
                Console.WriteLine($"\n{orderItem.Quantity} {orderItem.Code} ${totalPrice}");
                foreach (var item in packBreakDown)
                {
                    var currentPack = pastryType.Packs.First(pack => pack.PackSize == item.PackSize);
                    Console.WriteLine($"{item.PackCount} x {item.PackSize} ${currentPack.Price}");
                }
            }
            else
            {
                throw new Exception($"Order item for {orderItem.Quantity} {orderItem.Code} cannot be packed. Please manually review packing options for this order.");
            }
        }

        public List<List<LineItem>> FindCombos(int[] packSizes, int totalQuantity, List<LineItem> packCombo, int depth)
        {
            var packList = new List<List<LineItem>>();

            // iterate through the possible pack sizes
            for (int i = packSizes.Length - 1; i >= 0; i--)
            {
                // if the totalQuantity is larger than the pack size, then we can add a pack of that size to the combination
                if (totalQuantity >= packSizes[i])
                {
                    packCombo.Add(new LineItem
                    {
                        PackSize = packSizes[i],
                        PackCount = totalQuantity / packSizes[i]
                    });
                    var last = packCombo.Last();
                    var remaining = totalQuantity;
                    if (last != null) remaining -= (last.PackCount * last.PackSize);

                    // recursively call the method with the new totalQuantity to see if we can add more packs to this possible combination
                    packList.AddRange(FindCombos(packSizes, remaining, packCombo, depth + 1));
                }
                // if the totalQuantity is exactly 0 then have have made a combination that adds up to the required total
                else if (totalQuantity == 0)
                {
                    if (packCombo.Count > 0)
                    {
                        // add the successful combination to the list of possible packing combinations
                        packList.Add(new List<LineItem>(packCombo));
                        // Clear the combination so that we can try more.
                        packCombo.Clear();
                    }
                }
                //If the remaining totalQuantity is too small for any of the available pack sizes then the current combination has failed
                else if (packSizes.All(size => size > totalQuantity))
                {
                    // if we are at the bottom of the recursion
                    if (depth == packSizes.Length - 1)
                    {
                        var last = packCombo.Last();
                        // and we have tried all the possible pack sizes (tries with largest first, then smallest)
                        if (last.PackSize == packSizes[0])
                        {
                            //clear the current combination so we can start again
                            packCombo.Clear();
                        }
                        else
                        {
                            // we haven't tried all possible pack sizes yet so just remove the last one and keep trying with the remaining sizes
                            packCombo.RemoveAt(packCombo.Count - 1);
                        }
                    }
                    else
                    {
                        packCombo.Clear();
                    }

                    // return the current list of successful combinations.
                    return packList;
                }
            }
            return packList;
        }

        public void ProcessOrderBreakdown(List<OrderItem> order)
        {
            Console.WriteLine("Order Breakdown:");
            foreach (OrderItem item in order)
            {
                CalculateCost(item);
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please include your current inventory when you run Bakery.");
            }
            else
            {
                var littleBakeryCo = new Bakery(args[0]);

                Console.WriteLine("Enter file name of order you would like to process. Orders must be placed in the orders folder");
                var fileName = Console.ReadLine();
                var order = littleBakeryCo.LoadOrder(fileName);

                littleBakeryCo.ProcessOrderBreakdown(order);
            }
        }
    }
}