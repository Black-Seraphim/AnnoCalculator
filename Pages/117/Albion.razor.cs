using AnnoCalculator.Models;

namespace AnnoCalculator.Pages._117
{
    public partial class Albion
    {
        private const Belonging belonging = Belonging.Albium;

        private ProductionData? productionData;

        private Dictionary<string, int> citizenCounts = new();
        private Dictionary<string, double> citizenNeeds = new();
        private Dictionary<string, double> globalNeeds = new();
        private Dictionary<string, double> requiredBuildings = new();

        protected override async Task OnInitializedAsync()
        {
            productionData = await DataLoader.LoadDataAsync();

            if (productionData is not null)
            {
                citizenCounts = productionData.Citizens
                    .Where(c => c.Belonging == belonging.ToString())
                    .Select(c => c.Name)
                    .Distinct()
                    .ToDictionary(name => name, _ => 0);
            }
        }

        private void OnCitizenCountChanged(string citizenName, int newValue)
        {
            if (citizenCounts is null) return;

            citizenCounts[citizenName] = newValue;
            RecalculateCitizenNeeds();
            RecalculateRequiredBuilding();
        }

        private void RecalculateCitizenNeeds()
        {
            if (productionData is null) return;

            citizenNeeds.Clear();

            foreach (Citizen citizen in productionData.Citizens)
            {
                if (!citizenCounts.TryGetValue(citizen.Name, out int count) || count == 0) continue;

                foreach (var (good, perMinute) in citizen.Needs)
                {
                    if (citizenNeeds.TryGetValue(good, out double current))
                    {
                        citizenNeeds[good] = current + perMinute * count;
                    }
                    else
                    {
                        citizenNeeds[good] = perMinute * count;
                    }
                }
            }
        }

        private void RecalculateRequiredBuilding()
        {
            if (productionData is null) return;

            globalNeeds.Clear();
            requiredBuildings.Clear();

            Dictionary<string, double> pendingNeeds = new(citizenNeeds);

            while (pendingNeeds.Count > 0)
            {
                var current = pendingNeeds.First();
                var goodName = current.Key;
                var requiredAmount = current.Value;

                pendingNeeds.Remove(goodName);

                if (globalNeeds.ContainsKey(goodName))
                {
                    globalNeeds[goodName] += requiredAmount;
                }
                else
                {
                    globalNeeds[goodName] = requiredAmount;
                }

                Building? building = productionData.Buildings
                    .Where(b => b.Belonging == belonging.ToString())
                    .Where(b => b.Produces == goodName)
                    .FirstOrDefault();

                if (building is null) continue;

                var buildingName = building.Name;
                double requiredProductions = requiredAmount / (60.0 / building.SecondsToProduce);

                if (requiredBuildings.ContainsKey(buildingName))
                {
                    requiredBuildings[buildingName] += requiredProductions;
                }
                else
                {
                    requiredBuildings[buildingName] = requiredProductions;
                }

                foreach (var inputGood in building.Requires)
                {
                    double inputAmount = requiredProductions;

                    if (pendingNeeds.ContainsKey(inputGood))
                    {
                        pendingNeeds[inputGood] += inputAmount;
                    }
                    else
                    {
                        pendingNeeds[inputGood] = inputAmount;
                    }
                }
            }
        }

    }
}
