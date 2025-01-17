    ---NettoyeurDisque

NettoyeurDisque est une application de bureau développée en C# avec Visual Studio. Elle permet aux utilisateurs de scanner leur ordinateur pour identifier et supprimer les fichiers inutiles, libérant ainsi de l'espace disque.

**Fonctionnalités Principales

    Analyse du disque :
        Détection des fichiers temporaires, fichiers de cache et fichiers en double.
        Affichage des fichiers identifiés avec leurs tailles et emplacements.
    Nettoyage :
        Suppression sécurisée des fichiers sélectionnés par l'utilisateur.
        Suivi en temps réel via une barre de progression.
    Rapport de nettoyage :
        Résumé détaillé de l'espace libéré et statistiques par catégorie.
    Planification :
        Configuration de nettoyages automatiques réguliers (quotidien, hebdomadaire, mensuel).
    Interface utilisateur intuitive :
        Basée sur WPF avec tableau de bord et options configurables.

**Prérequis

    Système d'exploitation : Windows 10 ou version ultérieure.
    Framework requis : .NET Framework ou .NET Core compatible.
    Environnement de développement : Visual Studio (recommandé pour modifications).

**Installation

    Cloner le dépôt Git :

git clone https://github.com/MOUNIROUWAHAB23/NettoyeurDisque.git
cd NettoyeurDisque

Compiler le projet :

    Ouvrez le fichier de solution NettoyeurDisque.sln dans Visual Studio.
    Sélectionnez la configuration Release.
    Compilez le projet via le menu Build > Build Solution.


** Lancement du projet 
-lancer l'éxécutable dans bin/Release
ou

-Localiser l'exécutable : Une fois la compilation réussie, l'exécutable sera généré dans le dossier suivant :
ou

-NettoyeurDisque\bin\Release\NettoyeurDisque.exe ou NettoyeurDisque\bin\Release\NettoyeurDisque


ou télécharger le fichier ""Exécutable"" dans l'abrborescence .