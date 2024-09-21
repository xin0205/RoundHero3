using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RoundHero
{
    public partial class BattleCardManager : Singleton<BattleCardManager>
    {
        private List<float> cardPosList = new();
        public Dictionary<int, BattleCardEntity> CardEntities = new();
        public int PointerCardID = -1;

        public Data_Battle BattleData => DataManager.Instance.CurUser.GamePlayData.BattleData;

        public Data_BattlePlayer BattlePlayerData => BattlePlayerManager.Instance.BattlePlayerData;

        public System.Random Random;
        private int randomSeed;

        //public int CurSelectCardID;


        public void Init(int randomSeed)
        {
            this.randomSeed = randomSeed;
            Random = new System.Random(this.randomSeed);

        }

        public void Destory()
        {
            foreach (var kv in CardEntities)
            {
                GameEntry.Entity.HideEntity(kv.Value);
                
            }
            CardEntities.Clear();
        }

        public void InitCards()
        {
            BattlePlayerData.HandCards.Clear();
            BattlePlayerData.PassCards.Clear();
            BattlePlayerData.StandByCards.Clear();
            BattlePlayerData.ConsumeCards.Clear();

            var keyList = BattlePlayerManager.Instance.PlayerData.CardDatas.Keys.ToList();

            var randomPassCards = MathUtility.GetRandomNum(keyList.Count, 0, keyList.Count, Random);
            for (int i = 0; i < randomPassCards.Count; i++)
            {
                var cardID = keyList[randomPassCards[i]];
                var drCard = CardManager.Instance.GetCardTable(cardID);

                BattlePlayerData.StandByCards.Add(cardID);
            }
        }

        public void SetCardPosList(int cardCount)
        {
            cardPosList.Clear();

            var cardPosInterval = Constant.Battle.CardPosInterval;
            if (cardCount > Constant.Battle.ViewMaxHandCardCount)
            {
                cardPosInterval = Constant.Battle.CardPosInterval * (Constant.Battle.ViewMaxHandCardCount - 1) /
                                  (cardCount - 1);
            }

            if (cardCount % 2 == 0)
            {
                for (int i = -cardCount / 2; i < cardCount / 2; i++)
                {
                    cardPosList.Add(cardPosInterval / 2f + i * cardPosInterval);
                }
            }
            else
            {
                for (int i = -cardCount / 2; i <= cardCount / 2; i++)
                {
                    cardPosList.Add(0 + i * cardPosInterval);
                }
            }
        }

        public void RoundAcquireCards(bool firstRound = false)
        {
            if (BattleManager.Instance.CurUnitCamp != PlayerManager.Instance.PlayerData.UnitCamp)
                return;

            var cardCount = BattleCardManager.Instance.GetEachHardCardCount();
            var eachRoundAcquireCardCount =
                GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.EachRoundAcquireCard,
                    BattleManager.Instance.CurUnitCamp);
            if (eachRoundAcquireCardCount > 0)
            {
                var drEachRoundAcquireCard = GameEntry.DataTable.GetBless(EBlessID.EachRoundAcquireCard);

                cardCount += BattleBuffManager.Instance.GetBuffValue(drEachRoundAcquireCard.Values1[0]) * eachRoundAcquireCardCount;
            }

            var unuseCount =
                BattleManager.Instance.GetUnUseCardCount();

            BattleCardManager.Instance.AcquireCards(cardCount, firstRound, unuseCount);
        }

        public void AcquireCards(int cardCount, bool firstRound = false, int unuseCount = 0)
        {

            AcquireHardCard(cardCount, firstRound);

            AcquireCard(BattleController.Instance.StandByCardPos.position, unuseCount);

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }

        public int GetEachHardCardCount()
        {
            var eachHandCardCount = Constant.Battle.EachHardCardCount;

            var useCardNextRoundAcquireCardCount =
                GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.UseCardNextRoundAcquireCard,
                    BattleManager.Instance.CurUnitCamp);
            if (useCardNextRoundAcquireCardCount > 0)
            {
                var drUseCardNextRoundAcquireCard = GameEntry.DataTable.GetBless(EBlessID.UseCardNextRoundAcquireCard);

                var value0 = BattleBuffManager.Instance.GetBuffValue(drUseCardNextRoundAcquireCard.Values1[0]);
                var value1 = BattleBuffManager.Instance.GetBuffValue(drUseCardNextRoundAcquireCard.Values1[1]);
                if (BattlePlayerData.LastRoundUseCardCount <= value0)
                {
                    eachHandCardCount += value1 * 
                                         useCardNextRoundAcquireCardCount;
                }
            }

            if (BattleCurseManager.Instance.BattleCurseData.CurseIDs.Contains(ECurseID.MaxHandCardCount))
            {
                eachHandCardCount = eachHandCardCount > 5 ? 5 : eachHandCardCount;
            }

            return eachHandCardCount;
        }

        public async Task AcquireCard(Vector3 initPosition, int unuseCount = 0)
        {
            SetCardPosList(BattlePlayerData.HandCards.Count);

            for (int i = 0; i < BattlePlayerData.HandCards.Count; i++)
            {
                BattleCardEntity card;
                if (CardEntities.ContainsKey(BattlePlayerData.HandCards[i]))
                {
                    card = CardEntities[BattlePlayerData.HandCards[i]];
                    card.MoveCard(
                        new Vector3(cardPosList[i], BattleController.Instance.HandCardPos.localPosition.y, 0), 0.1f);
                    card.SetSortingOrder(i * 10);
                }
                else
                {
                    card = await GameEntry.Entity.ShowBattleCardEntityAsync(BattlePlayerData.HandCards[i]);

                    card.transform.position = initPosition;
                    card.SetSortingOrder(i * 10);
                    card.AcquireCard(new Vector2(cardPosList[i], BattleController.Instance.HandCardPos.localPosition.y),
                        i * 0.15f + 0.15f);

                    CardEntities.Add(card.BattleCardEntityData.CardID, card);
                }

                if (unuseCount > 0)
                {
                    card.BattleCardEntityData.CardData.UnUse = true;
                    unuseCount -= 1;
                }
            }

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            GameEntry.Event.Fire(null, RefreshCardInfoEventArgs.Create());
        }

        public List<int> ToPassCard()
        {
            var passCards = new List<int>();
            for (int i = BattlePlayerData.HandCards.Count - 1; i >= 0; i--)
            {
                var card = BattleManager.Instance.GetCard(BattlePlayerData.HandCards[i]);
                if (card.FuneCount(EBuffID.Spec_UnPass) > 0)
                    continue;

                passCards.Add(BattlePlayerData.HandCards[i]);
                BattlePlayerData.HandCards.RemoveAt(i);
                //BattleData.PassCards.Add(BattleData.HandCards[i]);
            }

            BattlePlayerData.PassCards.AddRange(passCards);


            return passCards;
        }

        public void ToPassCard(int cardID)
        {
            BattlePlayerData.HandCards.Remove(cardID);
            BattlePlayerData.PassCards.Add(cardID);

        }

         public void ToConsumeCard(int cardID)
        {
            BattlePlayerData.HandCards.Remove(cardID);
            BattlePlayerData.ConsumeCards.Add(cardID);

            var consumeCardAcquireNewCardCount =
                GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.ConsumeCardAddRandomCard,
                    BattleManager.Instance.CurUnitCamp);
            if (consumeCardAcquireNewCardCount > 0)
            {
                var drConsumeCardAcquireNewCard = GameEntry.DataTable.GetBless(EBlessID.ConsumeCardAddRandomCard);
                var value0 = BattleBuffManager.Instance.GetBuffValue(drConsumeCardAcquireNewCard.Values1[0]);
                var newCardIDs = AddRandomCard(value0);
                for (int i = 0; i < newCardIDs.Count; i++)
                {
                    var newCardID = newCardIDs[i];
                    GameUtility.DelayExcute(1f + 0.5f * i, () => { NewCardToHand(newCardID); });
                }

            }


        }

        public void ToStandByCard(int cardID)
        {
            BattlePlayerData.HandCards.Remove(cardID);
            BattlePlayerData.StandByCards.Add(cardID);

        }


        public void AcquireHardCard(int cardCount, bool firstRound = false)
        {
            if (firstRound)
            {
                var startFightAcquireCardCount =
                    GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.BattleStartAcquireCard,
                        BattleManager.Instance.CurUnitCamp);
                if (startFightAcquireCardCount > 0)
                {
                    var drUseCardNextRoundAcquireCard = GameEntry.DataTable.GetBless(EBlessID.BattleStartAcquireCard);
                    var value0 = BattleBuffManager.Instance.GetBuffValue(drUseCardNextRoundAcquireCard.Values1[0]);
                    cardCount += value0 * startFightAcquireCardCount;
                }

                for (int i = BattlePlayerData.StandByCards.Count - 1; i >= 0; i--)
                {
                    var cardID = BattlePlayerData.StandByCards[i];
                    var card = BattleManager.Instance.GetCard(cardID);
                    if (card.FuneCount(EBuffID.Spec_FirstRound) > 0)
                    {
                        BattlePlayerData.HandCards.Add(cardID);
                        BattlePlayerData.StandByCards.RemoveAt(i);
                        cardCount -= 1;
                    }
                }
            }

            if (cardCount <= 0)
                return;

            for (int i = 0; i < cardCount; i++)
            {
                if (BattlePlayerData.StandByCards.Count <= 0)
                {
                    if (BattlePlayerData.PassCards.Count > 0)
                    {
                        PassToStandBy();
                    }
                    else
                    {
                        break;
                    }

                }

                var cardID = BattlePlayerData.StandByCards[0];
                var card = BattleManager.Instance.GetCard(cardID);

                // if (card.FuneCount(EFuneID.InHand_AcquireCard) > 0)
                // {
                //     cardCount += 1;
                // }
                // else if (card.FuneCount(EFuneID.InHand_AddHeroCurHP) > 0)
                // {
                //     var drFune = GameEntry.DataTable.GetFune(EFuneID.InHand_AddHeroCurHP);
                //     FightManager.Instance.ChangeHP(FightManager.Instance.PlayerData.BattleHero, BattleBuffManager.Instance.GetBuffValue(drFune.BuffValues[0]),
                //         EHPChangeType.Unit, true, true);
                //     BattleManager.Instance.Refresh();
                // }

                BattlePlayerData.HandCards.Add(BattlePlayerData.StandByCards[0]);
                BattlePlayerData.StandByCards.RemoveAt(0);
            }
        }

        public void PassToStandBy()
        {
            var randomPassCards =
                MathUtility.GetRandomNum(BattlePlayerData.PassCards.Count, 0, BattlePlayerData.PassCards.Count);

            for (int i = 0; i < randomPassCards.Count; i++)
            {
                BattlePlayerData.StandByCards.Add(BattlePlayerData.PassCards[randomPassCards[i]]);
            }

            BattlePlayerData.PassCards.Clear();
        }

        public async void RandomStandByToPass()
        {
            var randomStandByCards = MathUtility.GetRandomNum(1, 0, BattlePlayerData.StandByCards.Count);

            for (int i = 0; i < randomStandByCards.Count; i++)
            {
                var cardID = BattlePlayerData.StandByCards[randomStandByCards[i]];
                BattlePlayerData.PassCards.Add(cardID);
                BattlePlayerData.StandByCards.Remove(cardID);

                var cardEntity = await GameEntry.Entity.ShowBattleCardEntityAsync(cardID);
                cardEntity.ShowInStandByCard();
                GameUtility.DelayExcute(0.5f + 0.5f * i, () => { cardEntity.ToPassCard(0.5f); });

            }



            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }

        public bool PreUseCard()
        {
            BattleManager.Instance.BattleState = EBattleState.TacticSelectUnit;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr = EBuffID.Spec_MoveUs.ToString();
                    
            return false;
        }

        public bool PreUseCard(int cardID)
        {
            var card = BattleManager.Instance.GetCard(cardID);
            DRCard drCard = null;
            drCard = GameEntry.DataTable.GetCard(card.CardID);

            var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardID);
            var cardType = drCard.CardType;

            // if (card.FuneDatas.Contains(EFuneID.AddCurHP) && cardEnergy > 0)
            // {
            //     cardEnergy -= 1;
            // }

            // if (CardManager.Instance.Contain(card.ID, EBuffID.Spec_UnRemove))
            // {
            //     return false;
            // }
            
            if (cardEnergy >
                BattleHeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurHP))
            {
                GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_NoEnergy));
                return false;
            }

            if (!BattlePlayerData.HandCards.Contains(cardID))
                return false;

            if (cardType == ECardType.Unit)
            {
                BattleManager.Instance.BattleState = EBattleState.UnitSelectGrid;
                //CardUseLine.gameObject.SetActive(true);
                // GameEntry.Event.Fire(null,
                //     RefreshCardUseTipsEventArgs.Create(true, Constant.Localization.Tips_SelectEnemy));
                //CurSelectCardID = cardID;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID = cardID;
                return false;

            }
            else if (cardType == ECardType.Tactic)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(drCard.BuffIDs[0]);
                var value = GameUtility.GetBuffValue(drCard.Values1[0]);
                
                // if (BattleCurseManager.Instance.IsTacticCardUnDamage(card.CardID) &&
                //     buffData.UnitAttribute == EUnitAttribute.CurHP && value < 0)
                // {
                //     return false;
                // }

                if (buffData.BuffTriggerType == EBuffTriggerType.SelectUnit || CardManager.Instance.Contain(card.ID, EBuffID.Spec_MoveUs))
                {
                    BattleManager.Instance.BattleState = EBattleState.TacticSelectUnit;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID = cardID;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr = buffData.BuffStr;
                    
                    return false;
                }
                else if (CardManager.Instance.Contain(card.ID, EBuffID.Spec_MoveGrid) || CardManager.Instance.Contain(card.ID, EBuffID.Spec_MoveAllGrid) )
                {
                    BattleManager.Instance.BattleState = EBattleState.MoveGrid;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr = buffData.BuffStr;
                    // GameEntry.Event.Fire(null,
                    //     RefreshCardUseTipsEventArgs.Create(true, Constant.Localization.Tips_MoveGrid));
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID = cardID;
                    return false;
                }
                else if (CardManager.Instance.Contain(card.ID, EBuffID.Spec_ExchangeGrid))
                {
                    //BattleManager.Instance.BattleState = EBattleState.TacticSelectGrid;
                    BattleManager.Instance.BattleState = EBattleState.ExchangeSelectGrid;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardID = cardID;
                    return false;
                }

            }

            return UseCard(cardID);
        }

        public bool UseCard(int cardID, int unitID = -1)
        {
            BattlePlayerData.RoundUseCardCount += 1;

            var card = BattleManager.Instance.GetCard(cardID);

            
            BattleBuffManager.Instance.TriggerBuff();
            BattleManager.Instance.TempTriggerData.TriggerType = ETempUnitType.Null;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.Clear();

            CardEntities.Remove(cardID);

            if (BattleManager.Instance.CurUnitCamp == PlayerManager.Instance.PlayerData.UnitCamp)
            {
                //Wrong
                // if (card.FuneCount(EFuneID.Use_ToStandBy) > 0)
                // {
                //     ToStandByCard(cardID);
                // }
                // else
                //card.FuneCount(EFuneID.EachRound_AddCurHP) > 0 || 
                if (card.IsUseConsume)
                {
                    ToConsumeCard(cardID);
                }
                else
                {
                    ToPassCard(cardID);
                }
            }

            
            BlessManager.Instance.EachUseCard(GamePlayManager.Instance.GamePlayData, cardID, unitID);

            BattleBuffManager.Instance.RecoverUseBuffState();
            FightManager.Instance.UseCardTrigger();
            BattleManager.Instance.Refresh();
            return true;

        }

        // public void ToHandCards(int cardID)
        // {
        //     if(BattleData.HandCards.Contains(cardID))
        //         return;
        //
        //     if (BattleData.StandByCards.Contains(cardID))
        //     {
        //         BattleData.StandByCards.Remove(cardID);
        //         BattleData.HandCards.Add(cardID);
        //     }
        //     else if (BattleData.PassCards.Contains(cardID))
        //     {
        //         BattleData.PassCards.Remove(cardID);
        //         BattleData.HandCards.Add(cardID);
        //     }
        //     
        // }

        

        public void ResetCardsPos(bool forceSortingOrder = false)
        {
            SetCardPosList(BattlePlayerData.HandCards.Count);

            var idx = 0;
            foreach (var kv in CardEntities)
            {
                kv.Value.SetSortingOrder(idx * 10, forceSortingOrder);
                kv.Value.MoveCard(
                    new Vector3(cardPosList[idx], BattleController.Instance.HandCardPos.localPosition.y, 0), 0.1f);
                kv.Value.transform.localScale = new Vector3(1f, 1f, 1f);
                idx += 1;
            }
            
        }

        public void Update()
        {
            if (BattleManager.Instance.BattleState == EBattleState.UnitSelectGrid ||
                BattleManager.Instance.BattleState == EBattleState.ExchangeSelectGrid ||
                BattleManager.Instance.BattleState == EBattleState.MoveGrid ||
                BattleManager.Instance.BattleState == EBattleState.MoveUnit ||
                BattleManager.Instance.BattleState == EBattleState.TacticSelectUnit ||
                BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)

            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (BattleManager.Instance.BattleState == EBattleState.MoveGrid)
                    {
                        BattleAreaManager.Instance.ResetMoveGrid();
                        //moveGridGO.SetActive(false);
                    }
                    else if (BattleManager.Instance.BattleState == EBattleState.MoveUnit)
                    {
                        BattleAreaManager.Instance.ShowBackupGrids(null);

                    }
                    else if (BattleManager.Instance.BattleState == EBattleState.ExchangeSelectGrid)
                    {
                        if (BattleAreaManager.Instance.TempExchangeGridData.GridPosIdx1 != -1 &&
                            BattleAreaManager.Instance.TempExchangeGridData.GridPosIdx2 != -1)
                        {
                            BattleAreaManager.Instance.ExchangeGrid(
                                BattleAreaManager.Instance.TempExchangeGridData.GridPosIdx1,
                                BattleAreaManager.Instance.TempExchangeGridData.GridPosIdx2);
                            
                        }

                        BattleAreaManager.Instance.TempExchangeGridData.GridPosIdx1 = -1;
                        BattleAreaManager.Instance.TempExchangeGridData.GridPosIdx2 = -1;
                        
                        BattleAreaManager.Instance.ShowBackupGrids(null);

                    }
                    else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
                    {
                        BattleManager.Instance.TempTriggerData.Reset();
                        BattleAreaManager.Instance.ShowBackupGrids(null);
                    }

                    BattleBuffManager.Instance.RecoverUseBuffState();
                    
                    BattleManager.Instance.Refresh();
                    
                }
            }
        }

        public float PassCards()
        {
            if(BattleManager.Instance.CurUnitCamp != PlayerManager.Instance.PlayerData.UnitCamp)
                return 0;
            
            var passCards = ToPassCard();

            var cardCount = CardEntities.Count;
            
            for (int i = 0; i < passCards.Count; i++)
            {
                CardEntities[passCards[i]].ToPassCard((cardCount - i) * 0.15f + 0.15f);
                CardEntities.Remove(passCards[i]);
            }

            ResetCardsPos(true);

            return cardCount * 0.15f + 0.15f;
        }
        
        public int GetCardEnergy(int cardID, int unitID = -1)
        {
            var card = BattleManager.Instance.GetCard(cardID);
            var drCard = GameEntry.DataTable.GetCard(card.CardID);

            var cardEnergy = drCard.Energy;

            if (unitID != -1)
            {
                var unitEntity = BattleUnitManager.Instance.GetUnitByID(unitID);
                switch (card.CardUseType)
                {
                    case ECardUseType.Raw:
                        cardEnergy = drCard.Energy;
                        break;
                    case ECardUseType.Attack:
                        //unitEntity.BattleUnit.RoundAttackTimes += 1;
                        cardEnergy = unitEntity.BattleUnit.RoundAttackTimes;
                        break;
                    case ECardUseType.Move:
                        //unitEntity.BattleUnit.RoundMoveTimes += 1;
                        cardEnergy = unitEntity.BattleUnit.RoundMoveTimes;
                        break;
            
                }

                cardEnergy = cardEnergy < 0 ? 0 : cardEnergy;

            }
            else
            {
                if (card.CardUseType == ECardUseType.Attack || card.CardUseType == ECardUseType.Move)
                {
                    cardEnergy = 0;
                }
                // if (BattleManager.Instance.TempTriggerData.UnitData is Data_BattleSolider solider &&
                // solider.CardID == cardID)
                // {
                //     cardEnergy = solider.Energy;
                //     
                // }
                
                cardEnergy += card.RoundEnergyDelta;
                // if (CardManager.Instance.Contain(card.ID, EBuffID.SelectUnit_Us_All_Atrb_HP) && CardManager.Instance.Contain(card.ID, EBuffID.SelectUnit_UsEnemy_Select_Atrb_HP))
                // {
                //     cardEnergy =
                //         BattleManager.Instance.BattleData.GetUnitCount(BattleManager.Instance.CurUnitCamp, new List<ERelativeCamp>() {ERelativeCamp.Us},
                //             new List<EUnitRole>() {EUnitRole.Staff, EUnitRole.Hero});
                // }

                var subEnergyCount = card.FuneCount(EBuffID.Spec_SubEnergy);
                if (subEnergyCount > 0 && cardEnergy > 0)
                {
                    cardEnergy -= subEnergyCount;
                    //cardEnergy = Math.Abs(cardEnergy);
                    cardEnergy = cardEnergy < 0 ? 0 : cardEnergy;

                }
                
                // var drCardEnergyMax = GameEntry.DataTable.GetCard(ECardID.CardEnergyMax);
                // var value = GameUtility.GetBuffValue(drCardEnergyMax.Values1[0]);
                
                // if (cardEnergy > value && BattlePlayerData.RoundBuffs.Contains(EBuffID.Spec_CardEnergyMax))
                // {
                //     cardEnergy = (int)value;
                // }

                if (BattleCurseManager.Instance.BattleCurseData.CurseIDs.Contains(ECurseID.OnGirdUnitAddEnergy) &&
                    BattleUnitManager.Instance.OnGridUnitContainCard(cardID))
                {
                    cardEnergy += 1;
                }
                
                if (BattleCurseManager.Instance.IsAddEnergyCard(drCard.CardType))
                {
                    cardEnergy += 1;
                }
                
                var eachUseCardUnUseEnergy = GamePlayManager.Instance.GamePlayData.GetUsefulBless(EBlessID.EachUseCardUnUseEnergy, BattleManager.Instance.CurUnitCamp);
                if (eachUseCardUnUseEnergy != null)
                {
                    eachUseCardUnUseEnergy.Value -= 1;
                    if (eachUseCardUnUseEnergy.Value <= 0)
                    {
                        var drBless = GameEntry.DataTable.GetBless(EBlessID.EachUseCardUnUseEnergy);
                        eachUseCardUnUseEnergy.Value = BattleBuffManager.Instance.GetBuffValue(drBless.Values1[0]);
                        cardEnergy = 0;
                    }
                }
            }

            

            return cardEnergy;
        }

        public async void PassCardToHand(int cardID)
        {
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardID);
            CardEntities.Add(card.BattleCardEntityData.CardID, card);
            card.PassCardToHand(0.5f);

            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
            
        }
        
        public async void NewCardToHand(int cardID)
        {
            BattlePlayerData.HandCards.Add(cardID);
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardID);
            CardEntities.Add(card.BattleCardEntityData.CardID, card);
            card.NewCardToHand(0.5f);

            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
        }

        public async void NewCardToPass(int cardID)
        {
            BattlePlayerData.PassCards.Add(cardID);
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardID);
            CardEntities.Add(card.BattleCardEntityData.CardID, card);
            card.NewCardToPass(0.5f);

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }
        
        public async void NewCardToStandBy(int cardID)
        {
            BattlePlayerData.StandByCards.Add(cardID);
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardID);
            CardEntities.Add(card.BattleCardEntityData.CardID, card);
            card.NewCardToStandBy(0.5f);

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            
        }
        
        public async void ToHandCards(int cardID)
        {
            if(BattlePlayerData.HandCards.Contains(cardID))
                return;
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardID);
            CardEntities.Add(card.BattleCardEntityData.CardID, card);
            
            
            if (BattlePlayerData.StandByCards.Contains(cardID))
            {
                BattlePlayerData.StandByCards.Remove(cardID);
                BattlePlayerData.HandCards.Add(cardID);
                card.StandByCardToHand(0.5f);
            }
            else if (BattlePlayerData.PassCards.Contains(cardID))
            {
                BattlePlayerData.PassCards.Remove(cardID);
                BattlePlayerData.HandCards.Add(cardID);
                card.PassCardToHand(0.5f);
            }

            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
            
        }
        
        public async void StandByCardToHand(int cardID)
        {
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardID);
            CardEntities.Add(card.BattleCardEntityData.CardID, card);
            card.StandByCardToHand(0.5f);

            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
            
        }

        public async void ConsumeCard(int cardID)
        {
            var unComsumeCardCount = GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.UnConsumeCard, BattleManager.Instance.CurUnitCamp);
            var isConsume = true;
            if (unComsumeCardCount > 0)
            {
                isConsume = Random.Next(0, 2) == 0;
            }

            BattleCardEntity cardEntity = null;
            
            if (BattlePlayerData.HandCards.Contains(cardID))
            {
                if (CardEntities.ContainsKey(cardID))
                {
                    cardEntity = CardEntities[cardID];
                    
                    CardEntities.Remove(cardID);
                    ResetCardsPos(true);
                }
                
                BattlePlayerData.HandCards.Remove(cardID);
                
            }
            else
            {
                cardEntity = await GameEntry.Entity.ShowBattleCardEntityAsync(cardID);
                if (BattlePlayerData.PassCards.Contains(cardID))
                {
                    cardEntity.ShowInPassCard();
                    BattlePlayerData.PassCards.Remove(cardID);
                }
                else if (BattlePlayerData.StandByCards.Contains(cardID))
                {
                    cardEntity.ShowInStandByCard();
                    BattlePlayerData.StandByCards.Remove(cardID);
                }
            }
            
            
            
            if (isConsume)
            {
                BattlePlayerData.ConsumeCards.Add(cardID);
                cardEntity.ToConsumeCard(0.5f);
                
            }
            else
            {
                BattlePlayerData.PassCards.Add(cardID);
                cardEntity.ToPassCard(0.5f);
            }

        }
        
        public void ConsumeCardForms()
        {
            var cards = new List<int>();
            cards.AddRange(BattlePlayerData.HandCards);
            cards.AddRange(BattlePlayerData.StandByCards);
            cards.AddRange(BattlePlayerData.PassCards);
            
            GameEntry.UI.OpenUIForm(UIFormId.CardsForm, new CardsFormData()
            {
                Cards = cards,
                SelectAction = (list) =>
                {
                    foreach (var consumeCardID in list)
                    {
                        ConsumeCard(consumeCardID);
                    }
                    
                    ResetCardsPos(true);
                    GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                },
                
                
                SelectCount = 1,
                Tips = "",
            });
        }
        
        public void AddNewCardForms()
        {
            var cards = new List<int>();
            CardManager.Instance.TempCards.Clear();
            for (int i = 0; i < 3; i++)
            {
                var tempID = CardManager.Instance.GetTempID();
                var randomCardID = 0;//(ECardID) Random.Next(0, Enum.GetNames(typeof(ECardID)).Length);
                CardManager.Instance.TempCards.Add(tempID, new Data_Card(tempID, randomCardID, new List<int>()));
            }
            
            cards.AddRange(CardManager.Instance.TempCards.Keys);

            GameEntry.UI.OpenUIForm(UIFormId.CardsForm, new CardsFormData()
            {
                Cards = cards,
                SelectAction = (list) =>
                {
                    foreach (var cardID in list)
                    {
                        NewCardToHand(cardID);
                    }
                    GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
                },
                
                SelectCount = 1,
                Tips = "",
            });
        }

        public List<int> AddRandomCard(int newCardCount)
        {
            //Wrong
            var cardIDList = new List<int>();//Enum.GetValues(typeof(ECardID)).OfType<ECardID>().ToList();
            
            foreach (var kv in CardManager.Instance.CardDatas)
            {
                if (!BattlePlayerData.ConsumeCards.Contains(kv.Key))
                {
                    cardIDList.Remove(kv.Value.CardID);
                }
            }

            var newCards = new List<int>();
            for (int i = 0; i < newCardCount; i++)
            {
                var randomIdx = Random.Next(0, cardIDList.Count);

                var tempID = AddTempNewCard(cardIDList[randomIdx]);
                newCards.Add(tempID);
                cardIDList.RemoveAt(randomIdx);
            }

            return newCards;
        }

        public int AddTempNewCard(int tempCardID)
        {
            var tempID = CardManager.Instance.GetTempID();
            CardManager.Instance.CardDatas.Add(tempID, new Data_Card(tempID,tempCardID, new List<int>()));
            return tempID;
        }

        public BattleCardEntity GetCardEntity(int cardID)
        {
            if (CardEntities.ContainsKey(cardID))
            {
                return CardEntities[cardID];
            }
            else
            {
                return null;
            }
        }

        public void RefreshSelectCard(int refreshCardID)
        {
            var idx = 0;
            var selectCardSiblingIdx = 999;
            if (CardEntities.ContainsKey(refreshCardID))
            {
                selectCardSiblingIdx = CardEntities[refreshCardID].RawSiblingIdx;
            }
            
            foreach (var kv in CardEntities)
            {
                if (kv.Key == refreshCardID)
                {
                    kv.Value.gameObject.GetComponent<RectTransform>().SetAsLastSibling();
                }
                else
                {
                    var siblingIdx = kv.Value.RawSiblingIdx;
                    if (siblingIdx > selectCardSiblingIdx)
                    {
                        siblingIdx -= 1;
                    }
                    kv.Value.gameObject.GetComponent<RectTransform>().SetSiblingIndex(siblingIdx);
                }
            }
        }
        
    }
}