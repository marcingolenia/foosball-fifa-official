namespace Foosball

open System

type NotEmptyString = internal NotEmptyString of string

module NotEmptyString =
    let create str =
      match String.IsNullOrWhiteSpace(str) with
      | true -> "Non-empty string is required." |> Error
      | _ -> NotEmptyString(str) |> Ok

    let value (NotEmptyString notEmptyString) = notEmptyString

