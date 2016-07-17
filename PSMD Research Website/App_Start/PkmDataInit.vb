Imports EntityFramework.BulkInsert.Extensions
Imports SkyEditor.ROMEditor.Windows
Imports SkyEditor.ROMEditor.Windows.FileFormats.PSMD

Public Module PkmDataInit
    Public Sub InitializeMissingData()
        Dim dirName = Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings("PSMDRawFilesDir"))
        If IO.Directory.Exists(dirName) Then
            Dim context As New DataContext
            'Open the game files
            Dim psmd As New PsmdDir
            psmd.OpenFile(dirName, New SkyEditor.Core.Windows.Providers.WindowsIOProvider)

            'Load abilities
            If context.Abilities.Count = 0 Then
                Dim abilities As New List(Of Ability)
                Dim abilityNames = IO.File.ReadAllLines(HttpContext.Current.Server.MapPath("~/alist.txt"))
                For count = 0 To abilityNames.Length - 1
                    abilities.Add(New Ability With {.ID = count, .Name = abilityNames(count)})
                Next
                context.BulkInsert(abilities)
                context.SaveChanges()
            End If


            'Load types
            If context.Types.Count = 0 Then
                Dim types As New List(Of PkmType)
                Dim typeNames = IO.File.ReadAllLines(HttpContext.Current.Server.MapPath("~/tlist.txt"))
                For count = 0 To typeNames.Length - 1
                    types.Add(New PkmType With {.ID = count, .Name = typeNames(count)})
                Next
                context.BulkInsert(types)
                context.SaveChanges()
            End If


            'Load moves
            If context.Moves.Count = 0 Then
                Dim moves As New List(Of Move)
                Dim moveNames = IO.File.ReadAllLines(HttpContext.Current.Server.MapPath("~/mlist.txt"))
                For count = 0 To moveNames.Length - 1
                    Dim m As New Move
                    m.ID = count
                    If String.IsNullOrEmpty(moveNames(count)) Then
                        m.Name = "(Unknown Move)"
                    Else
                        m.Name = moveNames(count)
                    End If
                    Dim waza = psmd.WazaData.Entries(count)
                    Dim actDataInfo = psmd.ActData.Entries(waza.ActDataInfoIndex)

                    With actDataInfo
                        m.EffectRate = .EffectRate
                        m.HPBellyChangeValue = .HPBellyChangeValue
                        m.TrapFlag = .TrapFlag
                        m.StatusChange = .StatusChange
                        m.StatChangeIndex = .StatChangeIndex
                        m.TypeChange = .TypeChange
                        m.TerrainChange = .TerrainChange
                        m.BaseAccuracy = .BaseAccuracy
                        m.MaxAccuracy = .MaxAccuracy
                        m.SizeTypeMove = .SizeTypeMove
                        m.TypeID = .TypeID
                        m.Attribute = .Attribute
                        m.BaseDamage = .BaseDamage
                        m.MaxDamage = .MaxDamage
                        m.BasePP = .BasePP
                        m.MaxPP = .MaxPP
                        m.CutsCorners = .CutsCorners
                        m.MoreTimeToAttack = .MoreTimeToAttack
                        m.TilesCount = .TilesCount
                        m.Range = .Range
                        m.Target = .Target
                        m.PiercingAttack = .PiercingAttack
                        m.SleepAttack = .SleepAttack
                        m.FaintAttack = .FaintAttack
                        m.NearbyDamage = .NearbyDamage
                        m.HitCounterIndex = .HitCounterIndex
                    End With
                    moves.Add(m)
                Next
                context.BulkInsert(moves)
                context.SaveChanges()
            End If

            'Load multihit
            If context.MoveMultiHits.Count = 0 Then
                Dim multi As New List(Of MoveMultiHit)
                For count = 0 To psmd.ActHitCountData.Entries.Count - 1
                    Dim m As New MoveMultiHit
                    With psmd.ActHitCountData.Entries(count)
                        m.HitChance2 = .HitChance2
                        m.HitChance3 = .HitChance3
                        m.HitChance4 = .HitChance4
                        m.HitChance5 = .HitChance5
                        m.HitCountMaximum = .HitCountMaximum
                        m.HitCountMinimum = .HitCountMinimum
                        m.ID = count
                        m.RepeatUntilMiss = .RepeatUntilMiss
                    End With
                    multi.Add(m)
                Next
                context.BulkInsert(multi)
                context.SaveChanges()
            End If

            'Load Pokemon
            If context.Pokemon.Count = 0 Then
                Dim pkms As New List(Of Pokemon)
                Dim pkmNames = IO.File.ReadAllLines(HttpContext.Current.Server.MapPath("~/plist.txt"))
                For count = 0 To pkmNames.Length - 1
                    Dim count2 = count
                    Dim pkm As New Pokemon With {.ID = count, .Name = pkmNames(count)}
                    Dim dataInfo = psmd.PokemonInfo.Entries(count2)
                    If dataInfo IsNot Nothing Then
                        With pkm
                            .DexNumber = dataInfo.DexNumber
                            .Category = dataInfo.Category
                            .ListNumber1 = dataInfo.ListNumber1
                            .ListNumber2 = dataInfo.ListNumber2
                            .BaseHP = dataInfo.BaseHP
                            .BaseAttack = dataInfo.BaseAttack
                            .BaseSpAttack = dataInfo.BaseSpAttack
                            .BaseDefense = dataInfo.BaseDefense
                            .BaseSpDefense = dataInfo.BaseSpDefense
                            .BaseSpeed = dataInfo.BaseSpeed
                            .EvolvesFromEntry = dataInfo.EvolvesFromEntry
                            .ExpTableNumber = dataInfo.ExpTableNumber
                            .Ability1 = dataInfo.Ability1
                            .Ability2 = dataInfo.Ability2
                            .AbilityHidden = dataInfo.AbilityHidden
                            .Type1 = dataInfo.Type1
                            .Type2 = dataInfo.Type2
                            .IsMegaEvolution = dataInfo.IsMegaEvolution
                            .MinEvolveLevel = dataInfo.MinEvolveLevel
                        End With
                        pkms.Add(pkm)
                    End If
                Next
                context.BulkInsert(pkms)
                context.SaveChanges()
            End If

            'Load levelup data
            If context.LevelUp.Count = 0 Then
                Dim levels As New List(Of PokemonLevelUp)
                For count = 0 To psmd.PokemonInfo.Entries.Count - 1
                    Dim pkm = psmd.PokemonInfo.Entries(count)
                    For count2 = 0 To pkm.MoveLevels.Length - 1
                        If pkm.Moves(count2) <> 0 Then
                            levels.Add(New PokemonLevelUp With {.PokemonID = count, .MoveID = pkm.Moves(count2), .Level = pkm.MoveLevels(count2)})
                        End If
                    Next
                Next
                context.BulkInsert(levels)
                context.SaveChanges()
            End If

            'Load experience data
            If context.Experience.Count = 0 Then
                Dim exps As New List(Of ExperienceLevel)
                For expTableNum = 0 To psmd.PokemonExpTable.Entries.Count - 1
                    For level = 0 To psmd.PokemonExpTable.Entries(expTableNum).Count - 1
                        Dim e = psmd.PokemonExpTable.Entries(expTableNum)(level)
                        exps.Add(New ExperienceLevel With {.ExperienceTableNumber = expTableNum, .Level = level, .Exp = e.Exp, .AddedAttack = e.AddedAttack, .AddedDefense = e.AddedDefense, .AddedHP = e.AddedHP, .AddedSpAttack = e.AddedSpAttack, .AddedSpDefense = e.AddedSpDefense, .AddedSpeed = e.AddedSpeed})
                    Next
                Next
                context.BulkInsert(exps)
                context.SaveChanges()
            End If

            'Load items
            If context.Items.Count = 0 Then
                Dim items As New List(Of Item)
                Dim itemNames = IO.File.ReadAllLines(HttpContext.Current.Server.MapPath("~/ilist.txt"))
                For count = 0 To psmd.ItemData.Entries.Count - 1
                    Dim i As New Item
                    With psmd.ItemData.Entries(count)
                        i.ID = count
                        i.BuyPrice = .BuyPrice
                        i.SellPrice = .SellPrice
                        If itemNames.Count > count Then
                            i.Name = itemNames(count)
                        End If
                        If String.IsNullOrWhiteSpace(i.Name) Then
                            i.Name = "(Unknown)"
                        End If
                        items.Add(i)
                    End With
                Next
                context.BulkInsert(items)
                context.SaveChanges()
            End If

            'Load dialog
            If context.GameStrings.Count = 0 Then
                Dim gameStrings As New List(Of GameString)

                For Each item In psmd.LanguageFiles
                    Dim lang As GameString.GameLanguage
                    Select Case item.Key
                        Case "message_en.bin"
                            lang = GameString.GameLanguage.EnglishEu
                        Case "message_us.bin"
                            lang = GameString.GameLanguage.EnglishUs
                        Case "message_fr.bin"
                            lang = GameString.GameLanguage.French
                        Case "message_ge.bin"
                            lang = GameString.GameLanguage.German
                        Case "message_it.bin"
                            lang = GameString.GameLanguage.Italian
                        Case "message_sp.bin"
                            lang = GameString.GameLanguage.Spanish
                        Case "message.bin"
                            lang = GameString.GameLanguage.Japanese
                    End Select

                    Dim dic = item.Value.GetFileDictionary

                    For Each header In item.Value.Header.FileData
                        Dim msgBin As New MessageBin
                        msgBin.EnableInMemoryLoad = True
                        msgBin.CreateFile(item.Value.GetFileData(header.Index))

                        For Each entry In msgBin.Strings
                            Dim gs As New GameString
                            gs.ID = entry.HashSigned
                            gs.Entry = entry.Entry
                            gs.Language = lang
                            gs.Filename = dic(header.FilenamePointer)
                            gameStrings.Add(gs)
                        Next

                    Next
                Next

                context.BulkInsert(gameStrings)
                context.SaveChanges()
            End If
        End If
    End Sub

End Module
