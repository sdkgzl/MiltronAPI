using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Shared.Enums
{
    /// <summary>
    /// Display -> Entity, Description -> Dto
    /// </summary>
    public enum EntityDtoMap
    {
        [Description("Shared.Dto.ModuleDto")]
        [Display(Name = "DataAccess.Entities.Module")]
        Module = 1,
    }
}
