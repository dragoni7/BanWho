using BanMe.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanMe.Application.Entities.Queries.GetRecordedGames;

public sealed record GetRecordedGames() : IQuery<RecordedGamesResponse>;
