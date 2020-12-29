module HttpContext

open System.Collections.Generic
open System.Threading.Tasks
open FakeItEasy
open Giraffe
open Giraffe.Serialization
open Microsoft.AspNetCore.Http
open System.IO
open System.Text
open Microsoft.Extensions.Primitives
open Microsoft.FSharpLu.Json
open Newtonsoft.Json

let next: HttpFunc = Some >> Task.FromResult

let buildMockHttpContext () =
    let emptyHeaders: IHeaderDictionary = HeaderDictionary(Dictionary<string, StringValues>()) :> IHeaderDictionary
    let customSettings = JsonSerializerSettings()
    customSettings.Converters.Add(CompactUnionJsonConverter(true))
    let context = A.Fake<HttpContext>()
    A.CallTo(fun () -> context.RequestServices.GetService(typeof<INegotiationConfig>))
     .Returns(DefaultNegotiationConfig()) |> ignore
    A.CallTo(fun () -> context.RequestServices.GetService(typeof<IJsonSerializer>))
     .Returns(NewtonsoftJsonSerializer(customSettings)) |> ignore
    A.CallTo(fun () -> context.Response.Headers).Returns(emptyHeaders) |> ignore
    context.Response.Body <- new MemoryStream()
    context

let writeToBody obj (httpContext: HttpContext) =
    let json = JsonConvert.SerializeObject(obj)
    let stream = new MemoryStream(Encoding.UTF8.GetBytes(json))
    httpContext.Request.Body <- stream
    httpContext

let toString(response: HttpResponse) =
    response.Body.Position <- 0L
    use reader = new StreamReader(response.Body, Encoding.UTF8)
    reader.ReadToEnd()

let deserializeResponse<'T> (response: HttpResponse) =
    response.Body.Position <- 0L
    use reader = new StreamReader(response.Body, Encoding.UTF8)
    let bodyJson = reader.ReadToEnd()
    let customSettings = JsonSerializerSettings()
    customSettings.Converters.Add(CompactUnionJsonConverter(true))
    JsonConvert.DeserializeObject<'T>(bodyJson, customSettings)
