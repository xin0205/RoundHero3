
using System;
using System.Collections.Generic;
using System.Linq;
using GameKit.Dependencies.Utilities;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public static partial class DataTableExtension
    {
        
        
        public static DRLocalization GetLocalization(this DataTableComponent dataTableComponent, string name)
        {
            var drLocalization = GameEntry.DataTable.GetDataTable<DRLocalization>();
            return drLocalization.GetDataRow((t) =>
            {
                return t.Name == name;
            });
        }
        
        public static DRScene GetScene(this DataTableComponent dataTableComponent, int sceneID)
        {
            var drScenes = GameEntry.DataTable.GetDataTable<DRScene>();
            return drScenes.GetDataRow((t) =>
            {
                return t.Id == sceneID;
            });
        }
        
        // public static DRCard GetCard(this DataTableComponent dataTableComponent, Data_Card cardData)
        // {
        //     
        //     
        //     return GetCard(dataTableComponent, cardData.CardID);
        //
        //     return null;
        //     
        // }
        
        public static DRCard GetCard(this DataTableComponent dataTableComponent, int cardID)
        {
            var drCards = GameEntry.DataTable.GetDataTable<DRCard>();
            return drCards.GetDataRow((t) =>
            {
                return t.Id == cardID;
            });

        }
        
        public static DRCard[] GetCards(this DataTableComponent dataTableComponent, ECardType cardType)
        {
            var drCards = GameEntry.DataTable.GetDataTable<DRCard>();
            return drCards.GetDataRows((t) =>
            {
                return t.CardType == cardType;
            });

        }
        
        public static DRCard[] GetCards(this DataTableComponent dataTableComponent, List<ECardType> cardTypes, int exceptCardID = -1)
        {
            var drCards = GameEntry.DataTable.GetDataTable<DRCard>();
            return drCards.GetDataRows((t) =>
            {
                foreach (var cardType in cardTypes)
                {
                    if (t.CardType == cardType && t.Id != exceptCardID)
                        return true;
                }
                
                return false;
            });

        }
        
        public static DRHero GetHero(this DataTableComponent dataTableComponent, EHeroID heroID)
        {
            var drHeros = GameEntry.DataTable.GetDataTable<DRHero>();
            return drHeros.GetDataRow((t) =>
            {
                return t.HeroID == heroID;
            });

        }
        
        public static DRHero GetHero(this DataTableComponent dataTableComponent, int id)
        {
            var drHeros = GameEntry.DataTable.GetDataTable<DRHero>();
            return drHeros.GetDataRow((t) =>
            {
                return t.Id == id;
            });

        }

        
        // public static DRCard GetCard(this DataTableComponent dataTableComponent, ETacticCardID tacticCardID)
        // {
        //     var drCards = GameEntry.DataTable.GetDataTable<DRCard>();
        //     return drCards.GetDataRow((t) =>
        //     {
        //         return t.CardID == tacticCardID;
        //     });
        //
        // }
        
       
        
        // public static DRCard GetTacticCard(this DataTableComponent dataTableComponent, EBuffID buffID)
        // {
        //     var drCards = GameEntry.DataTable.GetDataTable<DRCard>();
        //     return drCards.GetDataRow((t) =>
        //     {
        //         return t.BuffID == Enum.GetName(typeof(EBuffID), buffID);
        //     });
        // }
        
        public static DRBuff GetBuff(this DataTableComponent dataTableComponent, int buffID)
        {
            var drBuffs = GameEntry.DataTable.GetDataTable<DRBuff>();
            return drBuffs.GetDataRow((t) =>
            {
                return t.Id == buffID;
            });
        }
        
        public static DRBuff GetBuff(this DataTableComponent dataTableComponent, EBuffID buffID)
        {
            var drBuffs = GameEntry.DataTable.GetDataTable<DRBuff>();
            return drBuffs.GetDataRow((t) =>
            {
                return t.BuffIDs.Contains(buffID.ToString());
            });
        }
        
        public static List<DRBuff> GetBuffs(this DataTableComponent dataTableComponent, EBuffType buffType)
        {
            var drBuffs = GameEntry.DataTable.GetDataTable<DRBuff>();
            return drBuffs.GetDataRows((t) =>
            {
                return t.BuffTypes.Contains(buffType);
            }).ToList();
        }
        
        // public static DRBuff GetBuff(this DataTableComponent dataTableComponent, string buffID)
        // {
        //     var drBuffs = GameEntry.DataTable.GetDataTable<DRBuff>();
        //     return drBuffs.GetDataRow((t) =>
        //     {
        //         return t.BuffID == buffID;
        //     });
        // }
        
        // public static DRBuff GetBuff(this DataTableComponent dataTableComponent, EBuffID buffID)
        // {
        //     return GetBuff(dataTableComponent, buffID.ToString());
        // }
        
        // public static DRFune GetFune(this DataTableComponent dataTableComponent, int funeID)
        // {
        //     var drBuffs = GameEntry.DataTable.GetDataTable<DRFune>();
        //     return drBuffs.GetDataRow((t) =>
        //     {
        //         return t.Id == funeID;
        //     });
        // }
        
        // public static DRFune GetFune(this DataTableComponent dataTableComponent, EBuffID funeID)
        // {
        //     var drBuffs = GameEntry.DataTable.GetDataTable<DRFune>();
        //     return drBuffs.GetDataRow((t) =>
        //     {
        //         return t.BuffIDs.Contains(funeID.ToString());
        //     });
        // }
        
        public static DREnemy GetEnemy(this DataTableComponent dataTableComponent, int enemyID)
        {
            var drEnemies = GameEntry.DataTable.GetDataTable<DREnemy>();
            return drEnemies.GetDataRow((t) =>
            {
                return t.Id == enemyID;
            });
        }
        
        public static DRLink GetLink(this DataTableComponent dataTableComponent, ELinkID linkID)
        {
            var drLinks = GameEntry.DataTable.GetDataTable<DRLink>();
            return drLinks.GetDataRow((t) =>
            {
                return t.LinkID == linkID;
            });
        }
        
        public static DRBless GetBless(this DataTableComponent dataTableComponent, int ID)
        {
            var drLinks = GameEntry.DataTable.GetDataTable<DRBless>();
            return drLinks.GetDataRow((t) =>
            {
                return t.Id == ID;
            });
        }

        public static DRBless GetBless(this DataTableComponent dataTableComponent, EBlessID blessID)
        {
            var drLinks = GameEntry.DataTable.GetDataTable<DRBless>();
            return drLinks.GetDataRow((t) =>
            {
                return t.BlessID == blessID;
            });
        }
        
        public static DRGridProp GetGridProp(this DataTableComponent dataTableComponent, int gridPropID)
        {
            var drGridProps = GameEntry.DataTable.GetDataTable<DRGridProp>();
            return drGridProps.GetDataRow((t) =>
            {
                return t.Id == gridPropID;
            });
        }
        
        public static List<DREnemy> GetEnemys(this DataTableComponent dataTableComponent, EEnemyType enemyType)
        {
            var drEnemies = GameEntry.DataTable.GetDataTable<DREnemy>();
            return drEnemies.GetDataRows((t) =>
            {
                return  t.SpecBuffs[0]!= "None" && (enemyType == EEnemyType.Normal ? t.SpecBuffs[0] == "Empty" : t.SpecBuffs[0] != "Empty");
            }).ToList();
        }
        
    }
}