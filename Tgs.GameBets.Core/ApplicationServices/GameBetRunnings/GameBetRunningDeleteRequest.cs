using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Tgs.GameBets.Core.ApplicationServices.GameBetRunnings;
public record GameBetRunningDeleteRequest(
	Guid GameBetId) : IRequest;
