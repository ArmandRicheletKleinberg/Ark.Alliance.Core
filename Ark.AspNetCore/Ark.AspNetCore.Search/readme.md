## Ark.AspNetCore.Search
## Task List
| # | Task | Prompt (System / User) | Status | Remarks & Remaining Work |
|---|------|-----------------------|--------|-------------------------|
|1|Fix project reference to Ark.AspNetCore|User: "Fix all issue here :> do not mix Mvc libraries with WebAssembly Librairies"|Closed|Corrected csproj path to server project|
|2|Correct nested Ark.AspNetCore path|User: "Fix all issue here :> do not mix Mvc libraries with WebAssembly Librairies"|Closed|Updated project reference to avoid extra directory level|
### Purpose
This library is used to add an appplication wide search feature.
It works by generically querying the database depending of the types of the item to search.
This service can also be called from outside the application as a global search with more details.
***
### How to use
First creates the types of the item to search by inheriting the `SearchItemType<TEntity>` abstract class.  
This is a sample to search for material given the UM number :
```C#
public class MatiereNumeroUmSearchItemType : SearchItemType<MatiereDbEntity>
{
    public override string Code 
        => SearchItemTypeEnum.CommonMaterialUm.Code;

    public override Func<string> GetLabelFunc
        => SearchItemTypeEnum.CommonMaterialUm.GetLabelFunc;

    public override Func<string, Expression<Func<MatiereDbEntity, bool>>> WhereClauseFunc
        => text => matiere => matiere.NumeroUm.StartsWith(text) && matiere.SessionId == SessionCacheRepository.Get().ActiveSessionId;

    public override Expression<Func<MatiereDbEntity, SearchItem>> SelectItemFunc
        => matiere => new SearchItem(matiere.Id, matiere.NumeroUm);

    public override string FrontUrlSuffixPattern
        => "/matiere/{id}";

    public override Func<string> GetFrontUrlDescriptionFunc
        => () => "Détail de la matière N°{value}";

    public override Expression<Func<MatiereDbEntity, MatiereDbEntity>> SelectExtraDataFunc
        => matiere => new MatiereDbEntity
        {
            Id = matiere.Id,
            DateHeureDernierTraitement = matiere.DateHeureDernierTraitement,
            LibelleUm = matiere.LibelleUm,
            EtatUm = matiere.EtatUm,
            LibelleProprietaireMatiere = matiere.LibelleProprietaireMatiere,
            TypeSuspensGlobal = matiere.TypeSuspensGlobal,
            InfoUm = new MatiereInfoDbEntity
            {
                PoidsNet = matiere.InfoUm.PoidsNet
            },
            Commande = new CommandeDbEntity
            {
                NumeroCmdClientEtPoste = matiere.Commande.NumeroCmdClientEtPoste,
            }
        };

    public override Func<MatiereDbEntity, object> GetExtraIdFunc
        => commande => commande.Id;

    public override Func<MatiereDbEntity, DateTime?> GetExtraDateTimeFunc
        => matiere => matiere.DateHeureDernierTraitement;

    public override Func<MatiereDbEntity, string> GetExtraSummaryFunc
        => matiere => $"Produit {matiere.LibelleUm} - " +
                        $"Etat Sidérurgique {matiere.EtatUm} - " + 
                        $"Commande {matiere.Commande?.NumeroCmdClientEtPoste ?? "/"} - " +
                        $"Propriétaire {matiere.LibelleProprietaireMatiere} - " +
                        $"Poids net {matiere.InfoUm?.PoidsNet:0.000)}T - " +
                        $"Type de suspens  {matiere.TypeSuspensGlobal}";
}
```
Then finally adds the search feature to the application web host builder given the assemblies where to find the search item types.  
This will add automatically to the application controllers a new `/api/search` controller.
This controller will have CORS permission to be called from any origins. 
```C#
webBuilder.UseSearch<CockpitDbContext>(typeof(MatiereNumeroUmSearchItemType).Assembly)
```
***
### Properties to implement
#### Application scope search
##### Code
The code that uniquely identifies a search item type for all applications.  
This code is always prefixed by the application name or by COMMON_ if common type.  
A list of common type is defined in `SearchItemTypeEnum`.  
ie. `SearchItemTypeEnum.CommonOrderNumberWithItem.Code`.  
##### GetLabelFunc
The function used to get the search item type in the current culture.  
In most cases, should be `() => Texts.MySearchTextResourceKey`.
##### WhereClauseFunc
The function used to filter the entities to search.  
This generally will be `text => entity => entity.Property.StartsWith(text)`.
##### SelectItemFunc
The function used to get the identifier/value of the entity to search.  
This normally is generally the primary key with another value ie `entity => new KeyValuePair<string, string>(entity.Id, entity.Numero)`.
##### FrontUrlSuffixPattern
The front URL pattern suffix to navigate to.  
The pattern can specified a {id} or {value} format to be filled by the value.  
ie `material/{0}` with value G491158754 for navigating to http://myapplicationurl/material/G491158754.
##### GetFrontUrlDescriptionFunc
The description that describes the best the page/behavior where to navigate in the current culture.  
The pattern can specified a {id} or {value} format to be filled by the value.  
In most cases, should be `() => Texts.MySearchTextResourceKey`.
#### Global search
##### SelectExtraDataFunc
This expression is used to select all the data needed to extract the extra information about the item to search.  
This is used to restrain the data set fetch to enhance performance.
##### GetExtraIdFunc
This function is used to get the identifier of the searched entity used to match with the searched item.  
This is extracted from the extra data that have been selected previously in the `SelectExtraDataFunc` method.
##### GetExtraDateTimeFunc
This function is used to get the datetime of the searched entity.  
This is extracted from the extra data that have been selected previously in the `SelectExtraDataFunc` method.
##### GetExtraSummaryFunc
This function is used to get the summary text about the searched entity.  
This is extracted from the extra data that have been selected previously in the `SelectExtraDataFunc` method.
