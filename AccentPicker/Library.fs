namespace DnKR.AccentPicker

open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.FSharp.Control
open SkiaSharp

module AccentPicker =
    let private GetAccent (uri : string) = async {
        
        let colorArray: SKColor array =
            uri
            |> (new System.Net.Http.HttpClient()).GetByteArrayAsync
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> SKBitmap.Decode
            |> (fun b -> b.Pixels)
                     
        let ColorDict = Dictionary<SKColor, int>()
        
        for color in colorArray do
             if ColorDict.ContainsKey color then
                 ColorDict.Item color <- ColorDict.Item color + 1
             else
                 ColorDict.Add(color, 1)
             
        let maxColorCount: int = 
            ColorDict.Values
            |> seq
            |> Seq.max
            
        return
            ColorDict
            |> seq
            |> Seq.where (fun elm -> elm.Value = maxColorCount)
            |> Seq.item 0
            |> (fun r -> r.Key)
    }
    
    let public GetAccentString (uri:string) = task {
        return GetAccent(uri)
               |> Async.RunSynchronously
               |> (fun r -> r.ToString())
    }
    
    let public GetAccentBytes (uri: string) = task {
        return GetAccent(uri)
               |> Async.RunSynchronously
               |> (fun r -> [|r.Red, r.Green, r.Blue|])
    }
    