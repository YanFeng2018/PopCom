using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Contract
{
    public interface IBuildingRepository 
    {
        Building GetById(long id);
    }
}
