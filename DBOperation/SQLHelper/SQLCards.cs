using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBOperation.SQLHelper
{
    public class SQLCards
    {
        private static readonly object lockObj = new object(); 
        public static Cards GetChargeCards(string typeCode,decimal parValue)
        {
            try
            {
                lock (lockObj)
                {
                    ChargeAccountType AccountType = SQLChargeAccountType.GetChargeAccountType(p => p.Code == typeCode).FirstOrDefault();
                    Cards cards = GetCards(p => p.ChargeAccountTypeID == AccountType.ChargeAccountTypeID
                        && p.IsAvailable == true
                        && p.ReChargeStatus == 0
                        && p.Price == parValue).FirstOrDefault();

                    cards.ReChargeStatus = 1;

                    if (UpdateCards(cards))
                        return cards;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static List<Cards> GetCards(Func<Cards, bool> seleWhere)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    return dcl.Cards.Where(seleWhere).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CardNo">卡号</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="isUse">是否查询使用时间</param>
        /// <param name="type">卡类型</param>
        /// <param name="Status">充值状态</param>
        /// <returns></returns>
        public static List<Cards> GetCards(string CardNo, DateTime startTime, DateTime endTime,bool isUse=false , int type = -1, int Status = -1)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    var result = dcl.Cards.Where(p => p.CardNumber.Contains(CardNo));

                    if (isUse)
                        result = result.Where(p => p.UseTime >= startTime && p.UseTime <= endTime);
                    else
                        result = result.Where(p => p.CreatTime >= startTime && p.CreatTime <= endTime);
                    if (type != -1)
                        result = result.Where(p => p.ChargeAccountTypeID == type);
                    if (Status != -1)
                        result = result.Where(p => (p.ReChargeStatus != null && p.ReChargeStatus == Status));

                    return result.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static List<Cards> GetCards(DateTime startTime, DateTime endTime, bool isUse = false, int type = -1)
        {
            try
            {
                return GetCards("", startTime, endTime, isUse, type);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool AddCards(Cards cards)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    cards = dcl.Cards.Add(cards);
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool AddListCards(List<Cards> cardsSet)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    dcl.Cards.AddRange(cardsSet);
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateCards_ByMultiple(Cards card)
        {
            try
            {
                bool result = true;
                int updateCount = 0;
                card.UseTime = DateTime.Now;
                do
                {
                    if (updateCount > 5)
                        break;
                    result = SQLCards.UpdateCards(card);
                    System.Threading.Thread.Sleep(500);
                    updateCount++;
                }
                while (!result);
                return result;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public static bool UpdateCards_ByMultiple(Cards card, int cardStatus, string msg)
        {
            try
            {
                bool result = true;
                int updateCount = 0;
                card.ReChargeStatus = cardStatus;
                card.UseTime = DateTime.Now;
                card.ReChargeMsg += msg;
                do
                {
                    if (updateCount > 5)
                        break;
                    result = SQLCards.UpdateCards(card);
                    System.Threading.Thread.Sleep(500);
                    updateCount++;
                }
                while (!result);
                return result;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public static bool UpdateCards(Cards cards)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    dcl.Entry(cards).State = System.Data.Entity.EntityState.Modified;
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateCards(Cards cards, int cardStatus, string msg)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    cards.ReChargeStatus = cardStatus;
                    cards.UseTime = DateTime.Now;
                    cards.ReChargeMsg = msg;
                    dcl.Entry(cards).State = System.Data.Entity.EntityState.Modified;
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool DeleteCards(Cards cards)
        {
            try
            {
                using (DCLEntities dcl = new DCLEntities())
                {
                    dcl.Cards.Attach(cards);
                    dcl.Cards.Remove(cards);
                    return dcl.SaveChanges() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
