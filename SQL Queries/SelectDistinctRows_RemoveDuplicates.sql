/* 
	Delete duplicate rows. The columns observed are those listed
	after the 'partition by' clause.
*/
With repeatedRows as (
select *, ROW_NUMBER() Over(
partition by Sports, ProgrammingLanguages, Software
Order By dbo.Skills.Sports) As [rn]
From [IdeallyConnected.Experiments.AppICDbContext].[dbo].[Skills])
select * from repeatedRows where [rn] < 2
/*Delete repeatedRows where [rn] > 1 */