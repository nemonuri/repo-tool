
type ProjectSnapShot

type ProjectAction

type ProjectOption

module ProjectSnapShotTheory =

    val initialSnapShot : ProjectSnapShot

    val apply : ProjectSnapShot -> ProjectAction -> ProjectSnapShot

    val combine : ProjectAction -> ProjectAction -> ProjectAction

    val optionToAction : ProjectOption -> ProjectAction