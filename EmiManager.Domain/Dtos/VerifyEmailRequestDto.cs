using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmiManager.Domain.Dtos;

public class VerifyEmailRequestDto {
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
