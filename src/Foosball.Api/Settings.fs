module Settings

[<CLIMutable>]
type FoosballRules = { MaxSetPoints: byte; MaxSets: byte }

[<CLIMutable>]
type Settings =
  { FoosballRules: FoosballRules
    SqlConnectionString: string
    IdGeneratorSettings: IdGenerator.Settings }
