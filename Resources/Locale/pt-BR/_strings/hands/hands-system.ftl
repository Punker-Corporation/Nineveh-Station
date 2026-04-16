## HandsSystem

# Examine text after when they're holding something (in-hand)
comp-hands-examine = { CAPITALIZE(SUBJECT($user)) } contém { $items }.
comp-hands-examine-empty = { CAPITALIZE(SUBJECT($user)) } não contém nada.
comp-hands-examine-wrapper = [color=paleturquoise]{ $item }[/color]
hands-system-blocked-by = Mãos cheias
