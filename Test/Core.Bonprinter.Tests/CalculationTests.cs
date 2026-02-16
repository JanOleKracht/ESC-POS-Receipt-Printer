namespace Core.Bonprinter.Tests
{
    public class CalculationTests
    {
        [Fact]
        public void CalculateBruttoGesamt_ShouldCalculateCorrectly()
        {
            // Arrange
            var beleg = new Beleg
            {
                BelegPositionen = new List<BelegPosition>
                {
                    new BelegPosition {Menge=2,Brutto=20m, MwstZeichen="A" },
                    new BelegPosition {Menge=1,Brutto=10.5m, MwstZeichen="B" },
                }
            };

            // Act
            beleg.BerechneSumme();

            // Assert
            Assert.Equal(30.5m, beleg.BruttoGesamt);
        }

        [Fact]
        public void CalculateNettoGesamt_ShouldCalculateCorrectly()
        {
            // Arrange

            var beleg = new Beleg
            {
                BelegPositionen = new List<BelegPosition>
              {
                  new BelegPosition {Menge=2,Brutto=20m, MwstZeichen="A" },
                  new BelegPosition {Menge=1,Brutto=10.5m, MwstZeichen="B"},
                }
            };

            // Act
            beleg.BerechneSumme();

            // Assert
            Assert.Equal(26.62m, beleg.NettoGesamt);
        }

        [Fact]
        public void CalculateMwstBetragA_ShouldCalculateCorrectly()
        {
            // Arrange
            var beleg = new Beleg
            {
                BelegPositionen = new List<BelegPosition>
                {
                    new BelegPosition {Menge=2,Brutto=20m, MwstZeichen="A" },
                    new BelegPosition {Menge=1,Brutto=10.5m, MwstZeichen="B" },
                }
            };
            // Act
            beleg.BerechneSumme();
            // Assert
            Assert.Equal(3.19m, beleg.MwstBetragA);
        }

        [Fact]
        public void CalculateMwstBetragB_ShouldCalculateCorrectly()
        {
            // Arrange

            var beleg = new Beleg
            {
                BelegPositionen = new List<BelegPosition>
                {
                    new BelegPosition { Menge = 2, Brutto = 20m, MwstZeichen = "A" },
                    new BelegPosition { Menge = 1, Brutto = 10.5m, MwstZeichen = "B" },
                }
            };

            // Act
            beleg.BerechneSumme();

            // Assert
            Assert.Equal(0.69m, beleg.MwstBetragB);
        }

        [Fact]
        public void CalculateSumme_EmptyPositions_EverythingShouldBeZero()
        {
            //Arrange
            var beleg = new Beleg
            {
                BelegPositionen = new List<BelegPosition>()
            };
            //Act
            beleg.BerechneSumme();
            //Assert
            Assert.Equal(0m, beleg.BruttoGesamt);
            Assert.Equal(0m, beleg.NettoGesamt);
            Assert.Equal(0m, beleg.MwstBetragA);
            Assert.Equal(0m, beleg.MwstBetragB);
        }
    }
}