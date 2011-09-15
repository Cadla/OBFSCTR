#region

using System.Collections.Generic;
using System.Linq;
using JanuszLembicz.PW.BLC.Tests.Properties;
using JanuszLembicz.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace JanuszLembicz.PW.BLC.Tests
{
    /// <summary>
    ///This is a test class for BusinessLogicBusinessLogic_GetSmallestProducts and is intended
    ///to contain all BusinessLogicBusinessLogic_GetSmallestProducts Unit Tests
    ///</summary>
    [TestClass()]
    public class BusinessLogicBusinessLogic_GetSmallestProducts
    {
        private static IList<ProductListElement> _expectedCollection;
        private BusinessLogic_Accessor _businessLogic;

        [ClassInitialize()]
        public static void SetUpClass(TestContext context)
        {
            _expectedCollection = new List<ProductListElement>();

            _expectedCollection.Add(new ProductListElement()
                                        {
                                            Producer = "MOCK: Creative",
                                            Name = "MOCK: ZEN",
                                            HasDisplay = true,
                                            Interface = IOInterface.USB2,
                                            MemoryCapacity = 4096,
                                            Warranty = 2
                                        });

            _expectedCollection.Add(new ProductListElement()
                                        {
                                            Producer = "MOCK: Apple",
                                            Name = "MOCK: IPod",
                                            HasDisplay = false,
                                            Interface = IOInterface.USB1,
                                            MemoryCapacity = 512,
                                            Warranty = 1
                                        });

            _expectedCollection.Add(new ProductListElement()
                                        {
                                            Producer = "MOCK: Microsoft",
                                            Name = "MOCK: ZUNE",
                                            HasDisplay = true,
                                            Interface = IOInterface.USB2,
                                            MemoryCapacity = 8192,
                                        });
            _expectedCollection.Add(new ProductListElement()
                                        {
                                            Producer = "MOCK: Creative",
                                            Name = "MOCK: ZEN",
                                            HasDisplay = true,
                                            Interface = IOInterface.USB2,
                                            MemoryCapacity = 1024,
                                            Warranty = 3
                                        });

            _expectedCollection.Add(new ProductListElement()
                                        {
                                            Producer = "MOCK: Creative",
                                            Name = "MOCK: IPod",
                                            HasDisplay = true,
                                            Interface = IOInterface.USB2,
                                            MemoryCapacity = 512,
                                            Warranty = 2
                                        });

            _expectedCollection.Add(new ProductListElement()
                                        {
                                            Producer = "MOCK: Apple",
                                            Name = "MOCK: ZUNE",
                                            HasDisplay = true,
                                            Interface = IOInterface.USB2,
                                            MemoryCapacity = 1024,
                                            Warranty = 1
                                        });

            _expectedCollection = _expectedCollection.OrderBy(product => product.MemoryCapacity).ToList();
        }

        [ClassCleanup()]
        public static void TearDownClass()
        {
            _expectedCollection.Clear();
        }

        [TestInitialize]
        public void SetUp()
        {
            ObjectFactory.GetInstance.AssemblyName = Settings.Default.DAOFactoryDll;
            _businessLogic = new BusinessLogic_Accessor();
        }

        [TestMethod()]
        [DeploymentItem("JanuszLembicz.PW.BLC.dll")]
        public void TestStandardCaseAccuracy1()
        {
            var result = _businessLogic.GetSmallestProducts(1);
            Assert.AreEqual(2, result.Count);
            AssertResultContent(result, 2);
        }

        [TestMethod()]
        [DeploymentItem("JanuszLembicz.PW.BLC.dll")]
        public void TestStandardCaseAccuracy2()
        {
            var result = _businessLogic.GetSmallestProducts(2);
            Assert.AreEqual(2, result.Count);
            AssertResultContent(result, 2);
        }

        [TestMethod()]
        [DeploymentItem("JanuszLembicz.PW.BLC.dll")]
        public void TestStandardCaseAccuracy3()
        {
            var result = _businessLogic.GetSmallestProducts(3);
            Assert.AreEqual(4, result.Count);
            AssertResultContent(result, 4);
        }

        [TestMethod()]
        [DeploymentItem("JanuszLembicz.PW.BLC.dll")]
        public void TestStandardCaseAccuracy4()
        {
            var result = _businessLogic.GetSmallestProducts(4);
            Assert.AreEqual(4, result.Count);
            AssertResultContent(result, 4);
        }

        [TestMethod()]
        [DeploymentItem("JanuszLembicz.PW.BLC.dll")]
        public void TestEqseqoAccuracy()
        {
            var result = _businessLogic.GetSmallestProducts(6);
            Assert.AreEqual(6, result.Count);
            AssertResultContent(result, 6);
        }

        [TestMethod()]
        [DeploymentItem("JanuszLembicz.PW.BLC.dll")]
        public void TestZeroSelectionCountAccuracy()
        {
            var result = _businessLogic.GetSmallestProducts(0);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod()]
        [DeploymentItem("JanuszLembicz.PW.BLC.dll")]
        public void TestOutOfBoundSelectionAccuracy()
        {
            var result = _businessLogic.GetSmallestProducts(12);
            Assert.AreEqual(6, result.Count);
            AssertResultContent(result, 6);
        }

        private static void AssertResultContent(IList<ProductListElement> actual, int n)
        {
            for(int i = 0; i < n; i++)
            {
                Assert.AreEqual(_expectedCollection[i], actual[i]);
            }
        }
    }
}