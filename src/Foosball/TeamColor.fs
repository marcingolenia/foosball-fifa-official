namespace Foosball

// Official championship's colors
type TeamColor = Yellow | Black

module TeamColor =
    let create footballersColor =
      match footballersColor with
        | "yellow" -> Ok Yellow
        | "black" -> Ok Black
        | _ -> Error "Invalid footballers color; acceptable values are: yellow, black"

    let value = function Yellow -> "yellow" | _ -> "black"

