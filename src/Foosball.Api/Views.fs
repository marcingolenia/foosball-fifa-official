namespace Foosball.Api

module Views =
  open Giraffe.ViewEngine

  let index =
    html [] [
      head [] [
        title [] [ encodedText "Foosball API" ]
      ]
      body [] [
        h1 [] [ encodedText "Foosball API" ]
      ]
    ]
