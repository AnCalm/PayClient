using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChargeInterface.AntoInterface
{
    public interface ICharge
    {
        Order Charge(Order order);

        //void ChargeOrder(object obj);

    }
}
