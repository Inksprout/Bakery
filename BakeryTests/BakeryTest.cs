using BakeryProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace BakeryTests
{
    [TestClass]
    public class BakeryTests
    {
        private string inventory = "bakeryPriceList.json";

        [TestMethod]
        public void Bakery_Initialise_LoadsStock()
        {
            var littleBakeryCo = new Bakery(inventory);
            Assert.AreEqual(littleBakeryCo.Stock.Count, 3);
        }

        [TestMethod]
        public void Bakery_LoadOrder_LoadsAnOrder()
        {
            var littleBakeryCo = new Bakery(inventory);
            var order = littleBakeryCo.LoadOrder("customerOrder.json");
            Assert.AreEqual(order[0].Code, "VS5");
            Assert.AreEqual(order[1].Code, "MB11");
            Assert.AreEqual(order[2].Code, "CF");
        }

        [TestMethod]
        public void Bakery_CalculateCost_CalculatesTheCostOfAnOrderItem()
        {
            var testItem = new OrderItem { Code = "VS5", Quantity = 3 };
            var littleBakeryCo = new Bakery(inventory);

            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                littleBakeryCo.CalculateCost(testItem);
                var expected = "\n3 VS5 $6.99\r\n1 x 3 $6.99\r\n";
                Assert.AreEqual<string>(expected, sw.ToString());
            }
        }

        [TestMethod]
        public void Bakery_CalculateCost_ErrorsWhenTheOrderSizeCannotBePacked()
        {
            var testItem = new OrderItem { Code = "VS5", Quantity = 1 };
            var littleBakeryCo = new Bakery(inventory);
            var ex = Assert.ThrowsException<Exception>(() => littleBakeryCo.CalculateCost(testItem));

            Assert.AreEqual("Order item for 1 VS5 cannot be packed. Please manually review packing options for this order.", ex.Message);
        }

        [TestMethod]
        public void Bakery_processOrderBreakdown_CalculatesTheBreakdownForAFullOrder()
        {
            var order = new List<OrderItem>()
            {
                new OrderItem {Code = "VS5", Quantity = 10},
                new OrderItem {Code = "MB11", Quantity = 14},
                new OrderItem {Code = "CF", Quantity = 13}
            };

            var littleBakeryCo = new Bakery(inventory);
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                littleBakeryCo.ProcessOrderBreakdown(order);
                var expected = "Order Breakdown:\r\n\n10 VS5 $17.98\r\n2 x 5 $8.99\r\n\n14 MB11 $54.80\r\n1 x 8 $24.95\r\n3 x 2 $9.95\r\n\n13 CF $25.85\r\n2 x 5 $9.95\r\n1 x 3 $5.95\r\n";
                Assert.AreEqual(expected, sw.ToString());
            }
        }
    }
}