using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Cloners;
using Vagabond.Server.Config;

namespace Vagabond.Server.Services;


[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class BarterTraderService(ICloner cloner, DatabaseService databaseService)
{
    public void SetTraderUpdateTime(
        TraderConfig traderConfig,
        TraderBase traderBase,
        int minSeconds,
        int maxSeconds)
    {
        traderConfig.UpdateTime.Add(new UpdateTime
        {
            TraderId = traderBase.Id,
            Seconds = new MinMax<int>(minSeconds, maxSeconds)
        });
    }

    public void AddTraderWithEmptyAssortToDb(TraderBase traderBase)
    {
        databaseService.GetTables().Traders[traderBase.Id] = new Trader
        {
            Base = cloner.Clone(traderBase)!,
            Assort = new TraderAssort
            {
                Items = [],
                BarterScheme = new Dictionary<MongoId, List<List<BarterScheme>>>(),
                LoyalLevelItems = new Dictionary<MongoId, int>()
            },
            QuestAssort = new()
            {
                { "Started", new() },
                { "Success", new() },
                { "Fail", new() }
            },
            Dialogue = []
        };
    }

    public void AddTraderToLocales(TraderBase traderBase, string firstName, string description)
    {
        foreach (var (_, locale) in databaseService.GetTables().Locales.Global)
        {
            locale.AddTransformer(data =>
            {
                data![$"{traderBase.Id} FullName"] = traderBase.Name;
                data[$"{traderBase.Id} FirstName"] = firstName;
                data[$"{traderBase.Id} Nickname"] = traderBase.Nickname ?? string.Empty;
                data[$"{traderBase.Id} Location"] = traderBase.Location ?? string.Empty;
                data[$"{traderBase.Id} Description"] = description;
                return data;
            });
        }
    }

    public void AddTraderAssortment(Trader trader)
    {
        foreach (var barter in VagabondConfig._config.SpectatorTraderAssortment)
        {
            var rootId = new MongoId();
            trader.Assort.Items.Add(new Item
            {
                Id = rootId,
                Template = barter.Key,
                ParentId = "hideout",
                SlotId = "hideout",
                Upd = new Upd
                {
                    UnlimitedCount = true,
                    StackObjectsCount = 999999
                }
            });

            trader.Assort.BarterScheme[rootId] = [[]];

            foreach (var req in barter.Value)
            {
                trader.Assort.BarterScheme[rootId][0].Add(
                    new BarterScheme
                    {
                        Template = req.Key,
                        Count = req.Value
                    }
                );
            }

            trader.Assort.LoyalLevelItems[rootId] = 1;
        }
    }
}