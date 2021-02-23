namespace Arrangers

open System
open Foosball
open Foosball.Game

module A_team =

  let ``A team`` color: (TeamId * TeamColor) =
    let teamName = Guid.NewGuid().ToString()
                   |> NotEmptyString.create
                   |> function Ok value -> value | _ -> failwith "Cannot create team name"
    (teamName, color)

  let ``with Name`` name team =
    let teamName = name |> NotEmptyString.create
                        |> function Ok value -> value | _ -> failwith "Cannot create team name"
    (teamName, team |> snd)



