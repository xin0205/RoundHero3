using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace RoundHero
{
    public partial class BattleCardManager : Singleton<BattleCardManager>
    {
        public List<float> CardPosList { get;  } = new();
        public Dictionary<int, BattleCardEntity> CardEntities = new();
        public int PointerCardIdx = -1;
        public List<int> HandCardIdxs = new();

        public Data_Battle BattleData => DataManager.Instance.DataGame.User.CurGamePlayData.BattleData;

        public Data_BattlePlayer BattlePlayerData => BattlePlayerManager.Instance.BattlePlayerData;

        public System.Random Random;
        private int randomSeed;

        public int SelectCardIdx = -1;
        public int SelectCardHandOrder = -1;


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
            HandCardIdxs.Clear();
            SelectCardIdx = -1;
            SelectCardHandOrder = -1;
            PointerCardIdx = -1;
        }

        public void InitCards()
        {
            BattlePlayerData.HandCards.Clear();
            BattlePlayerData.PassCards.Clear();
            BattlePlayerData.StandByCards.Clear();
            BattlePlayerData.ConsumeCards.Clear();

            var keyList = BattlePlayerManager.Instance.PlayerData.CardDatas.Keys.ToList();
            var funeIdx = FuneManager.Instance.GetIdx();
            FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, 0));
            BattlePlayerManager.Instance.PlayerData.CardDatas[keyList[0]].FuneIdxs.Add(funeIdx);
            //BattlePlayerManager.Instance.PlayerData.CardDatas[keyList[0]].CardDestination = ECardDestination.Consume;

            // funeIdx = FuneManager.Instance.GetIdx();
            // FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, 31));
            // BattlePlayerManager.Instance.PlayerData.CardDatas[keyList[0]].FuneIdxs.Add(funeIdx);
            
            // funeIdx = FuneManager.Instance.GetIdx();
            // FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, 0));
            // BattlePlayerManager.Instance.PlayerData.CardDatas[keyList[1]].FuneIdxs.Add(funeIdx);
            //
            // funeIdx = FuneManager.Instance.GetIdx();
            // FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, 12));
            // BattlePlayerManager.Instance.PlayerData.CardDatas[keyList[1]].FuneIdxs.Add(funeIdx);
            // BattlePlayerManager.Instance.PlayerData.CardDatas[keyList[0]].MaxHPDelta += 10;

            foreach (var kv in BattlePlayerManager.Instance.PlayerData.CardDatas)
            {
                if (kv.Value.FuneCount(EBuffID.Spec_DoubleDmg) > 0)
                {
                    kv.Value.CardDestination = ECardDestination.Consume;
                    kv.Value.EnergyDelta += 1;
                    kv.Value.MaxHPDelta -= 1;
                    //kv.Value.DmgDelta
                }
            }
            
            // funeIdx = FuneManager.Instance.GetIdx();
            // FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, 26));
            // BattlePlayerManager.Instance.PlayerData.CardDatas[keyList[1]].FuneIdxs.Add(funeIdx);
            //
            // funeIdx = FuneManager.Instance.GetIdx();
            // FuneManager.Instance.FuneDatas.Add(funeIdx,new Data_Fune(funeIdx, 3));
            // BattlePlayerManager.Instance.PlayerData.CardDatas[keyList[1]].FuneIdxs.Add(funeIdx);

            var randomPassCards = MathUtility.GetRandomNum(keyList.Count, 0, keyList.Count, Random);
            for (int i = 0; i < randomPassCards.Count; i++)
            {
                var cardIdx = keyList[randomPassCards[i]];
                
                
                //var drCard = CardManager.Instance.GetCardTable(cardIdx);

                BattlePlayerData.StandByCards.Add(cardIdx);
            }
        }

        public void SetCardPosList(int cardCount)
        {
            CardPosList.Clear();

            var cardPosInterval = Constant.Battle.CardPosInterval;
            if (cardCount > Constant.Battle.ViewMaxHandCardCount)
            {
                cardPosInterval = Constant.Battle.CardPosInterval * (Constant.Battle.ViewMaxHandCardCount - 1) /
                                  (cardCount - 1);
            }

            var cardOrder = 0;

            if (cardCount % 2 == 0)
            {
                for (int i = -cardCount / 2; i < cardCount / 2; i++)
                {
                    if (cardOrder < SelectCardHandOrder)
                    {
                        CardPosList.Add(cardPosInterval / 2f + i * cardPosInterval - 0.45f * cardPosInterval);
                    }
                    else if (cardOrder > SelectCardHandOrder && SelectCardHandOrder != -1)
                    {
                        CardPosList.Add(cardPosInterval / 2f + i * cardPosInterval + 0.45f * cardPosInterval);
                    }
                    else
                    {
                        CardPosList.Add(cardPosInterval / 2f + i * cardPosInterval);
                    }

                    cardOrder++;
                }
                
            }
            else
            {
                for (int i = -cardCount / 2; i <= cardCount / 2; i++)
                {
                    if (cardOrder < SelectCardHandOrder)
                    {
                        CardPosList.Add(0 + i * cardPosInterval - 0.45f * cardPosInterval);
                    }
                    else if (cardOrder > SelectCardHandOrder && SelectCardHandOrder != -1)
                    {
                        CardPosList.Add(0 + i * cardPosInterval + 0.45f * cardPosInterval);
                    }
                    else
                    {
                        CardPosList.Add(0 + i * cardPosInterval);
                    }

                    cardOrder++;

                }
            }
        }

        public void RoundAcquireCards(bool firstRound = false)
        {
            // if (BattleManager.Instance.CurUnitCamp != PlayerManager.Instance.PlayerData.UnitCamp)
            //     return;

            var cardCount = BattleCardManager.Instance.GetEachHardCardCount();
            var eachRoundAcquireCardCount =
                GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.EachRoundAcquireCard,
                    BattleManager.Instance.CurUnitCamp);
            if (eachRoundAcquireCardCount > 0)
            {
                var drEachRoundAcquireCard = GameEntry.DataTable.GetBless(EBlessID.EachRoundAcquireCard);

                cardCount += (int)BattleBuffManager.Instance.GetBuffValue(drEachRoundAcquireCard.Values0[0]) * eachRoundAcquireCardCount;
            }

            var unuseCount =
                BattleManager.Instance.GetUnUseCardCount();

            BattleCardManager.Instance.AcquireCards(cardCount, firstRound, unuseCount);
        }

        public void AcquireCards(int cardCount, bool firstRound = false, int unuseCount = 0)
        {

            AcquireHardCard(cardCount, firstRound);

            AnimationAcquireCard(unuseCount);

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

                var value0 = BattleBuffManager.Instance.GetBuffValue(drUseCardNextRoundAcquireCard.Values0[0]);
                var value1 = BattleBuffManager.Instance.GetBuffValue(drUseCardNextRoundAcquireCard.Values0[1]);
                if (BattlePlayerData.LastRoundUseCardCount <= value0)
                {
                    eachHandCardCount += (int)value1 * 
                                         useCardNextRoundAcquireCardCount;
                }
            }

            if (BattleCurseManager.Instance.BattleCurseData.CurseIDs.Contains(ECurseID.MaxHandCardCount))
            {
                eachHandCardCount = eachHandCardCount > 5 ? 5 : eachHandCardCount;
            }

            return eachHandCardCount;
        }

        public async Task AnimationAcquireCard(int unuseCount = 0)
        {
            Log.Debug("AnimationAcquireCard:" + BattlePlayerData.HandCards.Count);
            SetCardPosList(BattlePlayerData.HandCards.Count);

            for (int i = 0; i < BattlePlayerData.HandCards.Count; i++)
            {
                var idx = i;
                BattleCardEntity card;
                if (CardEntities.ContainsKey(BattlePlayerData.HandCards[idx]))
                {
                    card = CardEntities[BattlePlayerData.HandCards[idx]];
                    card.BattleCardEntityData.HandSortingIdx = idx;
                    card.MoveCard(
                        new Vector3(CardPosList[idx], BattleController.Instance.HandCardPos.localPosition.y, 0), 0.1f);
                    card.SetSortingOrder(idx * 10);
                }
                else
                {
                    card = await GameEntry.Entity.ShowBattleCardEntityAsync(BattlePlayerData.HandCards[idx], idx);

                    card.transform.position = BattleController.Instance.StandByCardPos.position;
                    card.SetSortingOrder(idx * 10);
                    card.AcquireCard(new Vector2(CardPosList[idx], BattleController.Instance.HandCardPos.localPosition.y),
                         idx * 0.15f + 0.15f);

                    AddHandCard(card);
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
                //BattlePlayerData.PassCards.Add(BattlePlayerData.HandCards[i]);
            }

            BattlePlayerData.PassCards.AddRange(passCards);


            return passCards;
        }

        public void ToPassCard(int cardIdx)
        {
            BattlePlayerData.HandCards.Remove(cardIdx);
            BattlePlayerData.PassCards.Add(cardIdx);

        }

        

        public async Task ToStandByCards(int cardIdx)
        {
            BattlePlayerData.HandCards.Remove(cardIdx);
            var oriStandByCards = new List<int>(BattlePlayerData.StandByCards);
            BattlePlayerData.StandByCards.Clear();
            BattlePlayerData.StandByCards.Add(cardIdx);
            foreach (var standByCard in oriStandByCards)
            {
                BattlePlayerData.StandByCards.Add(standByCard);
            }

            // if(BattlePlayerData.StandByCards.Contains(cardIdx))
            //     return;
            //
            // var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            // AddHandCard(card);
            //
            //
            // if (BattlePlayerData.StandByCards.Contains(cardIdx))
            // {
            //     BattlePlayerData.StandByCards.Remove(cardIdx);
            //     BattlePlayerData.HandCards.Add(cardIdx);
            //     card.StandByCardToHand(0.5f);
            // }
            // else if (BattlePlayerData.PassCards.Contains(cardIdx))
            // {
            //     BattlePlayerData.PassCards.Remove(cardIdx);
            //     BattlePlayerData.HandCards.Add(cardIdx);
            //     card.PassCardToHand(0.5f);
            // }
            //
            // GameUtility.DelayExcute(0.5f, () =>
            // {
            //     ResetCardsPos(true);
            // });
        }

        public async Task AnimationConsumeToHand()
        {
            Log.Debug("AnimationConsumeToHand");
            if(BattlePlayerData.ConsumeCards.Count <= 0)
                return;
            
            Log.Debug("AnimationConsumeToHand2");
            var random = BattleCardManager.Instance.Random.Next(0, BattlePlayerData.ConsumeCards.Count);
            var cardIdx = BattlePlayerData.ConsumeCards[random];
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            AddHandCard(card);
            
            BattlePlayerData.ConsumeCards.Remove(cardIdx);
            BattlePlayerData.HandCards.Add(cardIdx);
            card.MoveCard(ECardPos.Consume, ECardPos.Hand, 0.5f); 

            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
        }
        
        public async Task AnimationToConsumeCards(int cardIdx)
        {
            if(BattlePlayerData.ConsumeCards.Contains(cardIdx))
                return;
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);

            if (BattlePlayerData.PassCards.Contains(cardIdx))
            {
                BattlePlayerData.PassCards.Remove(cardIdx);
                //card.PassCardToCenter(0.2f);
                card.MoveCard(ECardPos.Pass, ECardPos.Center); 
            }
            else if (BattlePlayerData.StandByCards.Contains(cardIdx))
            {
                BattlePlayerData.StandByCards.Remove(cardIdx);
                //card.StandByToCenter(0.2f);
                card.MoveCard(ECardPos.StandBy, ECardPos.Center); 
            }
            BattlePlayerData.ConsumeCards.Add(cardIdx);

            GameUtility.DelayExcute(0.2f, () =>
            {
                //card.ToConsumeCard(0.2f);
                
                card.MoveCard(ECardPos.Default, ECardPos.Consume); 
            });


        }
        
        public async Task AnimationToStandByCards(int cardIdx)
        {
            if(BattlePlayerData.StandByCards.Contains(cardIdx))
                return;
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            
            BattlePlayerData.PassCards.Remove(cardIdx);
            BattlePlayerData.HandCards.Add(cardIdx);
            //card.ToConsumeCard(0.5f);
            card.MoveCard(ECardPos.Default, ECardPos.Consume, 0.5f);
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
                    var value0 = BattleBuffManager.Instance.GetBuffValue(drUseCardNextRoundAcquireCard.Values0[0]);
                    cardCount += (int)value0 * startFightAcquireCardCount;
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
                var cardIdx = BattlePlayerData.StandByCards[randomStandByCards[i]];
                BattlePlayerData.PassCards.Add(cardIdx);
                BattlePlayerData.StandByCards.Remove(cardIdx);

                var cardEntity = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
                cardEntity.ShowInStandByCard();
                GameUtility.DelayExcute(0.5f + 0.5f * i, () =>
                {
                    //cardEntity.ToPassCard(0.5f);
                    cardEntity.MoveCard(ECardPos.Default, ECardPos.Pass, 0.5f);
                });

            }



            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }

        public bool PreUseCard()
        {
            BattleManager.Instance.SetBattleState(EBattleState.TacticSelectUnit);
            BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr = EBuffID.Spec_MoveUs.ToString();
                    
            return false;
        }

        public bool PreUseCard(int cardIdx)
        {
            var card = BattleManager.Instance.GetCard(cardIdx);
            DRCard drCard = null;
            drCard = GameEntry.DataTable.GetCard(card.CardID);

            var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardIdx);
            var cardType = drCard.CardType;

            // if (card.FuneDatas.Contains(EFuneID.AddCurHP) && cardEnergy > 0)
            // {
            //     cardEnergy -= 1;
            // }

            // if (CardManager.Instance.Contain(card.ID, EBuffID.Spec_UnRemove))
            // {
            //     return false;
            // }
            
            if (cardEnergy ==
                HeroManager.Instance.GetAllCurHP())
            {
                GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_HPNotUseAll));
                return false;
            }
            
            if (cardEnergy >
                HeroManager.Instance.GetAllCurHP())
            {
                GameEntry.UI.OpenMessage(GameEntry.Localization.GetString(Constant.Localization.Message_HPNotEnough));
                return false;
            }

            if (!BattlePlayerData.HandCards.Contains(cardIdx))
                return false;

            if (cardType == ECardType.Unit)
            {
                BattleManager.Instance.SetBattleState(EBattleState.UnitSelectGrid);
                //CardUseLine.gameObject.SetActive(true);
                // GameEntry.Event.Fire(null,
                //     RefreshCardUseTipsEventArgs.Create(true, Constant.Localization.Tips_SelectEnemy));
                //CurSelectCardID = cardID;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = cardIdx;
                return false;

            }
            if (cardType == ECardType.Prop)
            {
                BattleManager.Instance.SetBattleState(EBattleState.PropSelectGrid);

                BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = cardIdx;
                return false;

            }
            else if (cardType == ECardType.Tactic)
            {
                var buffData = BattleBuffManager.Instance.GetBuffData(drCard.BuffIDs[0]);
                var value = BattleBuffManager.Instance.GetBuffValue(drCard.Values0[0]);
                
                // if (BattleCurseManager.Instance.IsTacticCardUnDamage(card.CardID) &&
                //     buffData.UnitAttribute == EUnitAttribute.CurHP && value < 0)
                // {
                //     return false;
                // }

                if (buffData.BuffTriggerType == EBuffTriggerType.TacticSelectUnit ||
                    buffData.BuffTriggerType == EBuffTriggerType.SelectUnit ||
                    CardManager.Instance.Contain(card.CardIdx, EBuffID.Spec_MoveUs))
                {
                    BattleManager.Instance.SetBattleState(EBattleState.TacticSelectUnit);
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = cardIdx;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr = buffData.BuffStr;
                    
                    return false;
                }
                else if (buffData.BuffTriggerType == EBuffTriggerType.TacticSelectGrid)
                {
                    BattleManager.Instance.SetBattleState(EBattleState.TacticSelectGrid);
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = cardIdx;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr = buffData.BuffStr;
                    
                    return false;
                }
                else if (buffData.BuffTriggerType == EBuffTriggerType.TacticProp)
                {
                    BattleManager.Instance.SetBattleState(EBattleState.TacticSelectGrid);
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = cardIdx;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr = buffData.BuffStr;
                    
                    return false;
                }
                else if (CardManager.Instance.Contain(card.CardIdx, EBuffID.Spec_MoveGrid) || CardManager.Instance.Contain(card.CardIdx, EBuffID.Spec_MoveAllGrid) )
                {
                    BattleManager.Instance.SetBattleState(EBattleState.MoveGrid);
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.EnergyBuffData.BuffStr = buffData.BuffStr;
                    // GameEntry.Event.Fire(null,
                    //     RefreshCardUseTipsEventArgs.Create(true, Constant.Localization.Tips_MoveGrid));
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = cardIdx;
                    return false;
                }
                else if (CardManager.Instance.Contain(card.CardIdx, EBuffID.Spec_ExchangeGrid))
                {
                    BattleManager.Instance.SetBattleState(EBattleState.ExchangeSelectGrid);
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.TriggerBuffType = TriggerBuffType.Card;
                    BattleManager.Instance.TempTriggerData.TriggerBuffData.CardIdx = cardIdx;
                    return false;
                }

            }

            // bool ret = false;
            // if (cardEnergy >
            //     HeroManager.Instance.BattleHeroData.Attribute.GetAttribute(EHeroAttribute.CurHP))
            // {
            //     GameEntry.UI.OpenConfirm(new ConfirmFormParams()
            //     {
            //         Message = GameEntry.Localization.GetString(Constant.Localization.Message_ConfirmSubHeart),
            //         OnConfirm = () =>
            //         {
            //             ret = UseCard(cardID);
            //         }
            //     });
            //     
            //     
            // }
            // else
            // {
            //     ret = UseCard(cardID);
            // }
            
            var cardEntity = BattleCardManager.Instance.GetCardEntity(cardIdx);
            cardEntity.UseCardAnimation();
            return UseCard(cardIdx);
        }

        

        public bool UseCard(int cardIdx, int unitIdx = -1)
        {
            var cardEnergy = BattleCardManager.Instance.GetCardEnergy(cardIdx, unitIdx);
            
            
            
            var card = BattleManager.Instance.GetCard(cardIdx);
            

            BattleManager.Instance.RecordLastActionBattleData();
            
            BattlePlayerData.RoundUseCardCount += 1;

            //var card = BattleManager.Instance.GetCard(cardIdx);

            var cardEntity = BattleCardManager.Instance.GetCardEntity(cardIdx);
            RemoveHandCard(cardIdx);
            if (BattleManager.Instance.CurUnitCamp == PlayerManager.Instance.PlayerData.UnitCamp)
            {
                var carDestination = card.CardDestination;
                
                var unComsumeCardCount = GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.UnConsumeCard, BattleManager.Instance.CurUnitCamp);
                var isConsume = true;
                if (unComsumeCardCount > 0)
                {
                    carDestination = Random.Next(0, 2) == 0 ? card.CardDestination : ECardDestination.Consume;
                }
                
                //Wrong
                // if (card.FuneCount(EFuneID.Use_ToStandBy) > 0)
                // {
                //     ToStandByCard(cardID);
                // }
                // else
                //card.FuneCount(EFuneID.EachRound_AddCurHP) > 0 ||
                switch (carDestination)
                {
                    case ECardDestination.Pass:
                        ToPassCard(cardIdx);
                        break;
                    case ECardDestination.Consume:
                        ToConsumeCards(cardIdx);
                        break;
                    case ECardDestination.StandBy:
                        ToStandByCards(cardIdx);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            
            BlessManager.Instance.EachUseCard(GamePlayManager.Instance.GamePlayData, cardIdx, unitIdx);
            
            BattleBuffManager.Instance.TriggerBuff();
            
            if (BattlePlayerManager.Instance.BattlePlayerData.BattleBuffs.Contains(EBuffID.Spec_NextCardSubEnergy))
            {
                BattlePlayerManager.Instance.BattlePlayerData.BattleBuffs.Remove(EBuffID.Spec_NextCardSubEnergy);
            }

            foreach (var funeIdx in card.FuneIdxs)
            {
                var drBuff = FuneManager.Instance.GetBuffTable(funeIdx);
                foreach (var buffIDStr in drBuff.BuffIDs)
                {
                    var parseResult = Enum.TryParse<EBuffID>(buffIDStr, out EBuffID buffID);
                    if (parseResult)
                    {
                        switch (buffID)
                        {
                            case EBuffID.Spec_NextCardSubEnergy:
                                BattlePlayerManager.Instance.BattlePlayerData.BattleBuffs.Add(buffID);
                                break;    
                            default:
                                break;
                        }
                    }
                    
                }
            }
            
            BattleManager.Instance.TempTriggerData.TriggerType = ETempTriggerType.Null;
            BattleManager.Instance.TempTriggerData.TriggerBuffData.Clear();
            

            var moveParams = new MoveParams()
            {
                FollowGO = BattleController.Instance.HandCardPos.gameObject,
                DeltaPos = new Vector2(0, 0),
                IsUIGO = true,
            };
            
            var targetMoveParams = new MoveParams()
            {
                FollowGO = AreaController.Instance.UICore,
                DeltaPos = new Vector2(0, -25f),
                IsUIGO = true,
            };

            GameEntry.Entity.ShowBattleMoveValueEntityAsync(-cardEnergy, -cardEnergy, -1, false, false,
                moveParams,
                targetMoveParams);

            // var cardPos = cardEntity.transform.localPosition;
            // cardPos.y += 100;
            // var uiCorePos = AreaController.Instance.UICore.transform.localPosition;
            // uiCorePos.y -= 25f;
            
            //AQA
            // GameEntry.Entity.ShowBattleMoveValueEntityAsync(cardPos,
            //     uiCorePos,
            //     - cardEnergy, - cardEnergy, -1, false, false);
            
            BattleBuffManager.Instance.RecoverUseBuffState();
            BattleFightManager.Instance.UseCardTrigger();
            BattleManager.Instance.RefreshEnemyAttackData();
            
            HeroManager.Instance.UpdateCacheHPDelta();
            
            BattleCardManager.Instance.SetCardsPos();
            
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

        public void ResetCardUseType()
        {
            foreach (var kv in CardEntities)
            {
                kv.Value.BattleCardEntityData.CardData.CardUseType = ECardUseType.Raw;
                kv.Value.RefreshCardUseTypeInfo();
            }
        }
        

        public void ResetCardsPos(bool forceSortingOrder = false)
        {
            SetCardPosList(BattlePlayerData.HandCards.Count);

            var idx = 0;

            foreach (var cardIdx in HandCardIdxs)
            {
                if (!CardEntities.ContainsKey(cardIdx))
                {
                    continue;
                }
                

                var cardEntity = CardEntities[cardIdx];
                cardEntity.BattleCardEntityData.HandSortingIdx = idx;
                
                cardEntity.SetSortingOrder(idx * 10, forceSortingOrder);
                cardEntity.MoveCard(
                    new Vector3(CardPosList[idx], BattleController.Instance.HandCardPos.localPosition.y, 0), 0.1f);
                //cardEntity.transform.localScale = new Vector3(1f, 1f, 1f);
                cardEntity.ScaleCard(-1, 1, 0.01f);
                idx += 1;
                
            }

            
        }
        
        public void SetCardsPos()
        {
            SetCardPosList(BattlePlayerData.HandCards.Count);

            var idx = 0;

            foreach (var cardIdx in HandCardIdxs)
            {
                if (!CardEntities.ContainsKey(cardIdx))
                {
                    continue;
                }
                

                var cardEntity = CardEntities[cardIdx];
                cardEntity.BattleCardEntityData.HandSortingIdx = idx;

                var posy = BattleController.Instance.HandCardPos.localPosition.y;
                // || cardIdx == PointerCardIdx
                if (cardIdx == SelectCardIdx)
                    posy += Constant.Battle.SelectCardHeight;
                
                cardEntity.MoveCard(
                    new Vector3(CardPosList[idx], posy, 0), 0.1f);
                
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
                BattleManager.Instance.BattleState == EBattleState.TacticSelectGrid ||
                BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit ||
                BattleManager.Instance.BattleState == EBattleState.PropSelectGrid)

            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (TutorialManager.Instance.IsTutorial() && !TutorialManager.Instance.CheckTutorialEnd())
                        return;
                        
                    if (BattleManager.Instance.BattleState == EBattleState.MoveGrid)
                    {
                        if (BattleAreaManager.Instance.IsMoveGrid)
                        {
                            BattleAreaManager.Instance.ResetMoveGrid();
                            RefreshCardConfirm();
                        }
                        else
                        {
                            BattleBuffManager.Instance.RecoverUseBuffState();
                            RefreshCardConfirm();
                        }

                        //moveGridGO.SetActive(false);
                    }
                    else
                    {
                        if (BattleManager.Instance.BattleState == EBattleState.MoveUnit)
                        {
                            BattleAreaManager.Instance.ResetTmpUnitEntity();
                            BattleManager.Instance.TempTriggerData.Reset();
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
                            
                            BattleManager.Instance.SetBattleState(EBattleState.UseCard);
                            RefreshCardConfirm();

                        }
                        else if (BattleManager.Instance.BattleState == EBattleState.SelectHurtUnit)
                        {
                            BattleManager.Instance.TempTriggerData.Reset();
                            BattleAreaManager.Instance.ShowBackupGrids(null);
                        }
                        else if (BattleManager.Instance.BattleState == EBattleState.UnitSelectGrid)
                        {
                            BattleAreaManager.Instance.HideTmpUnitEntity();
                            BattleManager.Instance.TempTriggerData.Reset();
                            BattleAreaManager.Instance.ShowBackupGrids(null);
                        }
                        else if (BattleManager.Instance.BattleState == EBattleState.PropSelectGrid)
                        {
                            BattleAreaManager.Instance.HideTmpPropEntity();
                            BattleManager.Instance.TempTriggerData.Reset();
                            BattleAreaManager.Instance.ShowBackupGrids(null);
                        }
                        
                        BattleBuffManager.Instance.RecoverUseBuffState();

                    }

                    BattleManager.Instance.RefreshEnemyAttackData();
                    
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
                var time = (cardCount - i) * 0.15f + 0.15f;
                //CardEntities[passCards[i]].ToPassCard();
                var cardEntity = CardEntities[passCards[i]];
                RemoveHandCard(passCards[i]);
                cardEntity.MoveCard(ECardPos.Default, ECardPos.Pass, time);
               
            }

            ResetCardsPos(true);

            return cardCount * 0.15f + 0.15f;
        }

        public int GetCardMaxHP(int cardID, int cardIdx = -1)
        {

            var drCard = GameEntry.DataTable.GetCard(cardID);
            
            var maxHP = drCard.HP;
            var maxHPDelta = 0;
            if (cardIdx != -1)
            {
                var card = CardManager.Instance.GetCard(cardIdx);
                if (maxHP + card.MaxHPDelta < 0)
                {
                    card.MaxHPDelta = -maxHP;
                }
                maxHP += card.MaxHPDelta;
            }

            return maxHP;
        }
        
        public int GetCardEnergy(int cardIdx, int unitID = -1)
        {
            var card = BattleManager.Instance.GetCard(cardIdx);
            var drCard = GameEntry.DataTable.GetCard(card.CardID);

            var cardEnergy = drCard.Energy;

            if (unitID != -1)
            {
                var unitEntity = BattleUnitManager.Instance.GetUnitByIdx(unitID);
                switch (card.CardUseType)
                {
                    case ECardUseType.Raw:
                        cardEnergy = drCard.Energy;
                        break;
                    case ECardUseType.Attack:
                        //unitEntity.BattleUnit.RoundAttackTimes += 1;
                        cardEnergy = unitEntity.BattleUnitData.RoundAttackTimes;
                        break;
                    case ECardUseType.Move:
                        //unitEntity.BattleUnit.RoundMoveTimes += 1;
                        cardEnergy = unitEntity.BattleUnitData.RoundMoveTimes;
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
                    BattleUnitManager.Instance.OnGridUnitContainCard(cardIdx))
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
                        eachUseCardUnUseEnergy.Value = BattleBuffManager.Instance.GetBuffValue(drBless.Values0[0]);
                        cardEnergy = 0;
                    }
                }
            }

            
            
            var cardData = CardManager.Instance.GetCard(cardIdx);
                
            var drBuff = GameEntry.DataTable.GetBuff(EBuffID.Spec_SameUnitSubEnergy);
            if (cardData.FuneCount(EBuffID.Spec_SameUnitSubEnergy) > 0)
            {
                foreach (var kv in BattleUnitManager.Instance.BattleUnitDatas)
                {

                    if (kv.Value.UnitCamp == PlayerManager.Instance.PlayerData.UnitCamp
                        && kv.Value is Data_BattleSolider solider)
                    {
                        var soliderCard = CardManager.Instance.GetCard(solider.CardIdx);
                        if (soliderCard.CardID == cardData.CardID && solider.CardIdx != cardIdx)
                        {
                            if (cardEnergy > 0)
                            {
                                cardEnergy += (int)BattleBuffManager.Instance.GetBuffValue(drBuff.GetValues(0)[0]);

                            }
                                
                        }
                        
                    }
                    
                }
            }

            if (BattlePlayerManager.Instance.BattlePlayerData.BattleBuffs.Contains(EBuffID.Spec_NextCardSubEnergy))
            {
                var drBuff2 = GameEntry.DataTable.GetBuff(EBuffID.Spec_NextCardSubEnergy);
                if (cardEnergy > 0)
                {
                    cardEnergy += (int)BattleBuffManager.Instance.GetBuffValue(drBuff2.GetValues(0)[0]);
                }
            }
            
            
            
            if (cardEnergy + card.EnergyDelta < 0)
            {
                card.EnergyDelta = -cardEnergy;
            }

            cardEnergy += card.EnergyDelta;
            
            return cardEnergy;
        }

        public async void PassCardToHand(int cardIdx)
        {
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            AddHandCard(card);
            //card.PassCardToHand(0.5f);
            card.MoveCard(ECardPos.Pass, ECardPos.Hand, 0.5f);

            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
            
        }
        
        public async void NewCardToHand(int cardIdx)
        {
            BattlePlayerData.HandCards.Add(cardIdx);
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            AddHandCard(card);
            //card.NewCardToHand(0.5f);
            card.MoveCard(ECardPos.Center, ECardPos.Hand, 0.5f);
            
            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
        }

        public async void NewCardToPass(int cardIdx)
        {
            BattlePlayerData.PassCards.Add(cardIdx);
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            AddHandCard(card);
            //card.NewCardToPass(0.5f);
            card.MoveCard(ECardPos.Center, ECardPos.Pass, 0.5f);

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
        }
        
        public async void NewCardToStandBy(int cardIdx)
        {
            BattlePlayerData.StandByCards.Add(cardIdx);
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            //AddHandCard(card);
            //card.NewCardToStandBy(0.5f);
            card.MoveCard(ECardPos.Center, ECardPos.StandBy, 0.5f);

            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            
        }
        
        public async void AnimationToHandCards(int cardIdx)
        {
            if(BattlePlayerData.HandCards.Contains(cardIdx))
                return;
            
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            AddHandCard(card);
            
            
            if (BattlePlayerData.StandByCards.Contains(cardIdx))
            {
                BattlePlayerData.StandByCards.Remove(cardIdx);
                BattlePlayerData.HandCards.Add(cardIdx);
                //card.StandByCardToHand(0.5f);
                card.MoveCard(ECardPos.StandBy, ECardPos.Hand, 0.5f);
            }
            else if (BattlePlayerData.PassCards.Contains(cardIdx))
            {
                BattlePlayerData.PassCards.Remove(cardIdx);
                BattlePlayerData.HandCards.Add(cardIdx);
                //card.PassCardToHand(0.5f);
                card.MoveCard(ECardPos.Pass, ECardPos.Hand, 0.5f);
            }

            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
            
        }
        
        public async void StandByCardToHand(int cardIdx)
        {
            var card = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
            AddHandCard(card);
            
            //card.StandByCardToHand(0.5f);
            card.MoveCard(ECardPos.StandBy, ECardPos.Hand, 0.5f);

            GameUtility.DelayExcute(0.5f, () =>
            {
                ResetCardsPos(true);
            });
            
        }

        public void AddHandCard(BattleCardEntity cardEntity)
        {
            HandCardIdxs.Add(cardEntity.BattleCardEntityData.CardIdx);
            CardEntities.Add(cardEntity.BattleCardEntityData.CardIdx, cardEntity);
        }
        
        public void RemoveHandCard(int cardIdx)
        {
            HandCardIdxs.Remove(cardIdx);
            CardEntities.Remove(cardIdx);
        }
        
        public void ToConsumeCards(int cardIdx)
        {
            BattlePlayerData.HandCards.Remove(cardIdx);
            BattlePlayerData.ConsumeCards.Add(cardIdx);

            var consumeCardAcquireNewCardCount =
                GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.ConsumeCardAddRandomCard,
                    BattleManager.Instance.CurUnitCamp);
            if (consumeCardAcquireNewCardCount > 0)
            {
                var drConsumeCardAcquireNewCard = GameEntry.DataTable.GetBless(EBlessID.ConsumeCardAddRandomCard);
                var value0 = BattleBuffManager.Instance.GetBuffValue(drConsumeCardAcquireNewCard.Values0[0]);
                var newCardIdxs = AddRandomCard((int)value0);
                for (int i = 0; i < newCardIdxs.Count; i++)
                {
                    var newCardIdx = newCardIdxs[i];
                    GameUtility.DelayExcute(1f + 0.5f * i, () => { NewCardToHand(newCardIdx); });
                }

            }

            var addHP = BlessManager.Instance.ConsumeCardAddCurHP(GamePlayManager.Instance.GamePlayData);
            if (addHP > 0)
            {
                HeroManager.Instance.BattleHeroData.CurHP += addHP;
                // var startPos = BattleController.Instance.ConsumeCardPos.localPosition;
                // startPos.y += 25f;
                // var endPos = AreaController.Instance.UICore.transform.localPosition;
                // endPos.y -= 25f;
                //AQA
                // GameEntry.Entity.ShowBattleMoveValueEntityAsync(startPos,
                //     endPos,
                //     addHP, addHP, -1, false, false);
                
                var moveParams = new MoveParams()
                {
                    FollowGO = BattleController.Instance.ConsumeCardPos.gameObject,
                    DeltaPos = new Vector2(0, 25f),
                    IsUIGO = true,
                };
            
                var targetMoveParams = new MoveParams()
                {
                    FollowGO = AreaController.Instance.UICore,
                    DeltaPos = new Vector2(0, -25f),
                    IsUIGO = true,
                };

                GameEntry.Entity.ShowBattleMoveValueEntityAsync(addHP, addHP, -1, false, false,
                    moveParams,
                    targetMoveParams);
                
            }
            
            
            
            GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());

        }

        // public async void ConsumeCard(int cardIdx)
        // {
        //     var unComsumeCardCount = GamePlayManager.Instance.GamePlayData.BlessCount(EBlessID.UnConsumeCard, BattleManager.Instance.CurUnitCamp);
        //     var isConsume = true;
        //     if (unComsumeCardCount > 0)
        //     {
        //         isConsume = Random.Next(0, 2) == 0;
        //     }
        //
        //     BattleCardEntity cardEntity = null;
        //     
        //     if (BattlePlayerData.HandCards.Contains(cardIdx))
        //     {
        //         if (CardEntities.ContainsKey(cardIdx))
        //         {
        //             cardEntity = CardEntities[cardIdx];
        //             
        //             RemoveHandCard(cardIdx);
        //             ResetCardsPos(true);
        //         }
        //         
        //         BattlePlayerData.HandCards.Remove(cardIdx);
        //         
        //     }
        //     else
        //     {
        //         cardEntity = await GameEntry.Entity.ShowBattleCardEntityAsync(cardIdx);
        //         if (BattlePlayerData.PassCards.Contains(cardIdx))
        //         {
        //             cardEntity.ShowInPassCard();
        //             BattlePlayerData.PassCards.Remove(cardIdx);
        //         }
        //         else if (BattlePlayerData.StandByCards.Contains(cardIdx))
        //         {
        //             cardEntity.ShowInStandByCard();
        //             BattlePlayerData.StandByCards.Remove(cardIdx);
        //         }
        //     }
        //     
        //     
        //     
        //     if (isConsume)
        //     {
        //         BattlePlayerData.ConsumeCards.Add(cardIdx);
        //         //cardEntity.ToConsumeCard(0.5f);
        //         cardEntity.MoveCard(ECardPos.Default, ECardPos.Consume, 0.5f);
        //         
        //         
        //
        //     }
        //     else
        //     {
        //         BattlePlayerData.PassCards.Add(cardIdx);
        //         //cardEntity.ToPassCard(0.5f);
        //         cardEntity.MoveCard(ECardPos.Default, ECardPos.Pass, 0.5f);
        //     }
        //
        // }
        
        public void ConsumeCardForms()
        {
            // var cards = new List<int>();
            // cards.AddRange(BattlePlayerData.HandCards);
            // cards.AddRange(BattlePlayerData.StandByCards);
            // cards.AddRange(BattlePlayerData.PassCards);
            //
            // GameEntry.UI.OpenUIForm(UIFormId.CardsForm, new CardsFormData()
            // {
            //     Cards = cards,
            //     SelectAction = (list) =>
            //     {
            //         foreach (var consumeCardID in list)
            //         {
            //             ConsumeCard(consumeCardID);
            //         }
            //         
            //         ResetCardsPos(true);
            //         GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            //     },
            //     
            //     
            //     SelectCount = 1,
            //     Tips = "",
            // });
        }
        
        public void AddNewCardForms()
        {
            // var cards = new List<int>();
            // CardManager.Instance.TempCards.Clear();
            // for (int i = 0; i < 3; i++)
            // {
            //     var tempID = CardManager.Instance.GetTempIdx();
            //     var randomCardID = 0;//(ECardID) Random.Next(0, Enum.GetNames(typeof(ECardID)).Length);
            //     CardManager.Instance.TempCards.Add(tempID, new Data_Card(tempID, randomCardID, new List<int>()));
            // }
            //
            // cards.AddRange(CardManager.Instance.TempCards.Keys);
            //
            // GameEntry.UI.OpenUIForm(UIFormId.CardsForm, new CardsFormData()
            // {
            //     Cards = cards,
            //     SelectAction = (list) =>
            //     {
            //         foreach (var cardID in list)
            //         {
            //             NewCardToHand(cardID);
            //         }
            //         GameEntry.Event.Fire(null, RefreshBattleUIEventArgs.Create());
            //     },
            //     
            //     SelectCount = 1,
            //     Tips = "",
            // });
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

                var tempCardIdx = AddTempNewCard(cardIDList[randomIdx]);
                newCards.Add(tempCardIdx);
                cardIDList.RemoveAt(randomIdx);
            }

            return newCards;
        }

        public int AddTempNewCard(int tempCardID)
        {
            var tempIdx = CardManager.Instance.GetTempIdx();
            CardManager.Instance.CardDatas.Add(tempIdx, new Data_Card(tempIdx, tempCardID, new List<int>()));
            return tempIdx;
        }

        public BattleCardEntity GetCardEntity(int cardIdx)
        {
            if (CardEntities.ContainsKey(cardIdx))
            {
                return CardEntities[cardIdx];
            }
            else
            {
                return null;
            }
        }

        public void RefreshSelectCard()
        {
            var idx = 0;
            // var selectCardSiblingIdx = 999;
            // if (CardEntities.ContainsKey(refreshCardID))
            // {
            //     selectCardSiblingIdx = CardEntities[refreshCardID].RawSiblingIdx;
            // }
            
            foreach (var kv in CardEntities)
            {
                if (kv.Key == BattleCardManager.Instance.SelectCardIdx)
                {
                    kv.Value.SetSortingOrder(1000);
                    //kv.Value.gameObject.GetComponent<RectTransform>().SetAsLastSibling();
                }
                else
                {
                    // var siblingIdx = kv.Value.RawSiblingIdx;
                    // if (siblingIdx > selectCardSiblingIdx)
                    // {
                    //     siblingIdx -= 1;
                    // }
                    //kv.Value.gameObject.GetComponent<RectTransform>().SetSiblingIndex(siblingIdx);
                    kv.Value.SetSortingOrder(kv.Value.BattleCardEntityData.HandSortingIdx * 10);
                }

                idx++;
            }
        }

        public void RefreshCurCardEnergy(int cardEnergy)
        {
            if (CardEntities.ContainsKey(SelectCardIdx))
            {
                CardEntities[SelectCardIdx].RefreshEnergy(cardEnergy);
            }
        }
        
        public void UnSelectCard()
        {
            
            foreach (var kv in CardEntities)
            {
                //|| PointerCardIdx == kv.Value.BattleCardEntityData.CardData.CardIdx
                if (SelectCardIdx == kv.Value.BattleCardEntityData.CardData.CardIdx)
                {
                    kv.Value.UnSelectCard();
                    SelectCardIdx = -1;
                    SelectCardHandOrder = -1;
                }
            }
            PointerCardIdx = -1;
            
        }

        public void RefreshCardConfirm()
        {
            if(!CardEntities.ContainsKey(SelectCardIdx))
                return;
            
            CardEntities[SelectCardIdx].RefreshCofirm();
        }

        public void ShowTacticDownMulti(List<int> gridPosIdxs)
        {
            foreach (var gridPosIdx in gridPosIdxs)
            {

                GameEntry.Entity.ShowCommonEffectEntityAsync("TacticDownMultiEntity",
                    GameUtility.GridPosIdxToPos(gridPosIdx), EColor.Blue);
            }
        }
        
    }
}