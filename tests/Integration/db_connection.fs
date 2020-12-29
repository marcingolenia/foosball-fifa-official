module DbConnection

open Foosball.PostgresPersistence

let create =
  DapperFSharp.createSqlConnection
    "Host=localhost;User Id=postgres;Password=Secret!Passw0rd;Database=foosball;Port=5432"
