ammonia-smell = Algo cheira forte!!
perishable-1 = [color=green]{ CAPITALIZE(OBJECT($target)) } o corpo ainda parece fresco.[/color]
perishable-2 = [color=orangered]{ CAPITALIZE(OBJECT($target)) } o corpo não parece particularmente fresco.[/color]
perishable-3 = [color=red]{ CAPITALIZE(OBJECT($target)) } o corpo não parece nada novo.[/color]
perishable-1-nonmob = [color=green]{ CAPITALIZE(SUBJECT($target)) } parece ainda novo.[/color]
perishable-2-nonmob = [color=orangered]{ CAPITALIZE(SUBJECT($target)) } não parece particularmente novo.[/color]
perishable-3-nonmob = [color=red]{ CAPITALIZE(SUBJECT($target)) } não parece particularmente novo.[/color]
rotting-rotting = [color=orange]{ CAPITALIZE(SUBJECT($target)) } { GENDER($alvo) -> 
    [male] гниёт
    [female] гниёт
    [epicene] гниют
   *[neuter] гниёт
 }![/color]
rotting-bloated = [color=orangered]{ CAPITALIZE(SUBJECT($target)) } { GENDER($alvo) -> 
    [male] вздулся
    [female] вздулась
    [epicene] вздулись
   *[neuter] вздулось
 }![/color]
rotting-extremely-bloated = [color=red]{ CAPITALIZE(SUBJECT($target)) } forte { GENDER($alvo) -> 
    [male] вздулся
    [female] вздулась
    [epicene] вздулись
   *[neuter] вздулось
 }![/color]
rotting-rotting-nonmob = [color=orange]{ CAPITALIZE(SUBJECT($target)) } está apodrecendo![/color]
rotting-bloated-nonmob = [color=orangered]{ CAPITALIZE(SUBJECT($target)) } inchado![/color]
rotting-extremely-bloated-nonmob = [color=red]{ CAPITALIZE(SUBJECT($target)) } muito inchado![/color]
