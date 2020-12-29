namespace Foosball.Api

type Link = { Rel: string; Href: string }
type LinkedResult<'a> = { Result: 'a; Links: Link list }
type ScoreDto = { Color: string; Team: string }
type NewGameDto = { Team1: string; Team2: string }
