using System;
using FluentAssertions;
using AglService.Contracts;
using AglWpfApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AglWpfAppUnitTest
{
    [TestClass]
    public class PeopleViewModelUnitTest
    {
        private string jsonRawData = "[{\"name\":\"Bob\",\"gender\":\"Male\",\"age\":23,\"pets\":[{\"name\":\"Garfield\",\"type\":\"Cat\"},{\"name\":\"Fido\",\"type\":\"Dog\"}]},{\"name\":\"Jennifer\",\"gender\":\"Female\",\"age\":18,\"pets\":[{\"name\":\"Garfield\",\"type\":\"Cat\"}]},{\"name\":\"Steve\",\"gender\":\"Male\",\"age\":45,\"pets\":null},{\"name\":\"Fred\",\"gender\":\"Male\",\"age\":40,\"pets\":[{\"name\":\"Tom\",\"type\":\"Cat\"},{\"name\":\"Max\",\"type\":\"Cat\"},{\"name\":\"Sam\",\"type\":\"Dog\"},{\"name\":\"Jim\",\"type\":\"Cat\"}]},{\"name\":\"Samantha\",\"gender\":\"Female\",\"age\":40,\"pets\":[{\"name\":\"Tabby\",\"type\":\"Cat\"}]},{\"name\":\"Alice\",\"gender\":\"Female\",\"age\":64,\"pets\":[{\"name\":\"Simba\",\"type\":\"Cat\"},{\"name\":\"Nemo\",\"type\":\"Fish\"}]}]";
        private Moq.Mock<IAglPeopleService> AglService;

        private IList<AglDto.Person> expectedPeople;

        [TestInitialize]
        public void Init()
        {
            this.AglService = new Moq.Mock<IAglPeopleService>();
            expectedPeople = JsonConvert.DeserializeObject<List<AglDto.Person>>(jsonRawData);

            this.AglService.Setup(m => m.GetAll()).Returns(async () =>
            {
                return await Task.Run(() => { return expectedPeople; });
            });
        }

        [TestMethod]
        public void CreateViewTestMethod()
        {
            // Arrange
            PeopleViewModel peopleViewModel = this.GetViewModel();

            // Assert
            Assert.IsNull(peopleViewModel.People);
        }

        [TestMethod]
        public async Task InitializeViewTestMethod()
        {
            // Arrange
            PeopleViewModel peopleViewModel = this.GetViewModel();

            // Act
           await peopleViewModel.Initialize();

            // Assert
            Assert.IsNotNull(peopleViewModel.People);
        }

        [TestMethod]
        public async Task MustHaveSixPetOwnersTestMethod()
        {
            // Arrange
            PeopleViewModel peopleViewModel = this.GetViewModel();

            // Act
            await peopleViewModel.Initialize();

            // Assert
            Assert.AreEqual(6, peopleViewModel.People.Count);
        }

        [TestMethod]
        public async Task FemaleMustHave3PetsTestMethod()
        {
            // Arrange
            PeopleViewModel peopleViewModel = this.GetViewModel();

            // Act
            await peopleViewModel.Initialize();

            // Assert
            Assert.AreEqual(3, peopleViewModel.AllFemaleOwnerCats.Count);
        }

        [TestMethod]
        public async Task MaleMustHave4PetsTestMethod()
        {
            // Arrange
            PeopleViewModel peopleViewModel = this.GetViewModel();

            // Act
            await peopleViewModel.Initialize();

            // Assert
            Assert.AreEqual(4, peopleViewModel.AllMaleOwnerCats.Count);
        }

        private PeopleViewModel GetViewModel()
        {
            return new PeopleViewModel(AglService.Object);
        }
    }
}
