namespace SuaveRestApi

module App = 
    open SuaveRestApi.Rest
    open Suave.Http
    open Suave.Web
    open SuaveRestApi.Db
    open SuaveRestApi.MusicStoreDb
    open Suave.Http.Applicatives
    
    type AudienceDto = 
        { AudienceId : string
          Name : string }
    
    type JwResource<'a> = 
        { GetAll : unit -> 'a list }
    
    type JwInfo = 
        { Single : string
          Length : float }
    
    [<EntryPoint>]
    let main argv = 
        let personWebPart = 
            rest "people" { GetAll = Db.getPeople
                            GetById = Db.getPerson
                            Create = Db.createPerson
                            Update = Db.updatePerson
                            UpdateById = Db.updatePersonById
                            Delete = Db.deletePerson
                            IsExists = Db.isPersonExists }
        
        let albumWebPart = 
            rest "albums" { GetAll = MusicStoreDb.getAlbums
                            GetById = MusicStoreDb.getAlbumById
                            Create = MusicStoreDb.createAlbum
                            Update = MusicStoreDb.updateAlbum
                            UpdateById = MusicStoreDb.updateAlbumById
                            Delete = MusicStoreDb.deleteAlbum
                            IsExists = MusicStoreDb.isAlbumExists }
        
        let myRest myPath resource = 
            let resourcePath = sprintf "/%s" myPath
            choose [ path resourcePath >>= choose [ GET >>= warbler (fun _ -> resource.GetAll() |> JSON) ] ]
        
        let getJwInfo() = 
            [ { JwInfo.Single = "Hello"
                Length = 3.3 } ]
        
        let jwWebPart = myRest "jw" { JwResource.GetAll = getJwInfo }
        let app = choose [ personWebPart; albumWebPart; jwWebPart ]
        startWebServer defaultConfig app
        0
