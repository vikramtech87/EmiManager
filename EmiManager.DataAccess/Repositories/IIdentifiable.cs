using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmiManager.DataAccess.Repositories {
    public interface IIdentifiable {
        public string Id { get; }
    }
}
