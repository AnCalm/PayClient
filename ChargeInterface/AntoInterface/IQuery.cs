using EntityDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeInterface.AntoInterface
{
    public interface IQuery
    {
        Order Query(Order order);
    }
}
