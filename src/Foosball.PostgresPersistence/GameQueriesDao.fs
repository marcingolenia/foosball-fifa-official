namespace Foosball.PostgresPersistence

open DapperFSharp
open Foosball.Application

module GameQueriesDao =
    [<Literal>]
    let ReadBy =
      "
select
    Id,
    (data -> 1 -> 'Rules' ->> 'MaxSets')::int as MaxSets,
    (data -> 1 -> 'Rules' ->> 'MaxSetPoints')::int as MaxSetPoints,
    (data -> 1 ->> 'StartedAt')::timestamp as StartedAt,
    (data -> 1 ->> 'FinishedAt')::timestamp as FinishedAt,
    data -> 1 -> 'Teams' -> 0 ->> 1 as Team1,
    data -> 1 -> 'Teams' -> 1 ->> 1 as Team2
from games
where Id = @Id;
select
  rn::int as setNo,
	setScore -> 'By' -> 0 ->> 1  as Team,
	setScore -> 'By' ->> 1  as scoredWith,
  (setScore ->> 'At')::timestamp as scoredAt
from games
, json_array_elements(data -> 1 -> 'Score') WITH ORDINALITY AS score(gameScore, rn)
, json_array_elements(gameScore) setScore
where Id = @Id;
"
    let ListQuery = "
with allScores as (
select
    Id,
    (data -> 1 -> 'Rules' ->> 'MaxSetPoints')::int as MaxSetPoints,
    setScore -> 'By' -> 0 ->> 1 as team, rn as setNo
from games
, json_array_elements(data -> 1 -> 'Score') WITH ORDINALITY AS score(gameScore, rn)
, json_array_elements(gameScore) setScore
)

, setsWinners as
(
select Id, team, setNo
from allScores
group by (Id, MaxSetPoints, team, setNo)
having count(*) = MaxSetPoints
)

, gameScores as
(
select Id, team, COUNT(team) as score
from setsWinners
group by Id ,team
)

, games as (
select
    Id,
    (data -> 1 ->> 'StartedAt')::timestamp as StartedAt,
    (data -> 1 ->> 'FinishedAt')::timestamp as FinishedAt,
    data -> 1 -> 'Teams' -> 0 ->> 1 as Team1,
    data -> 1 -> 'Teams' -> 1 ->> 1 as Team2
from games
)
select games.*, coalesce(team1Scores.score, 0)::int as team1Score, coalesce(team2Scores.score, 0)::int as team2Score from games
left join gameScores team1Scores on team1Scores.id = games.id and team1Scores.team = games.team1
left join gameScores team2Scores on team2Scores.id = games.id and team2Scores.team = games.team2
order by startedAt desc"

    let readBy createConnection (id: int64) =
        async {
            use! connection = createConnection ()
            let! resultsSets = connection |> sqlQueryMultiple ReadBy {|id = id|}
            let! dbGame = resultsSets.ReadSingleAsync<DbModels.GameDetails>() |> Async.AwaitTask
            let! scores = resultsSets.ReadAsync<Queries.Score>() |> Async.AwaitTask
            return dbGame.toQueryResult (scores |> List.ofSeq)
        }

    let list createConnection =
        async {
            use! connection = createConnection ()
            return! connection |> sqlQuery<Queries.GameOverview> ListQuery
        }



