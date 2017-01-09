# PillBlasta
This is the third-person shooter game as instructed by [Sebastian Lague][Seb] in 
his video series.

The player is a Unity [capsule collider][CapsuleCollider] (the Pill) which 
shoots (the Blasta part) enemy capsule colliders that follow and attack the 
player. As in an actual game, both the player and the enemy objects have finite
health and can die.

This is a comprehensive video tutorial. It covers all conceivable aspects of
video game production which includes asset creation, artificial intelligence,
map generation and progressive leveling.

My code differs in style from Sebastian's with an emphasis on readability and 
immutability.

# Recommendations

### Don't start off with this video series
If you are completely new to Unity then I would not start with this video 
series. The author, Sebastian Lague, speeds through his tutorials with the 
assumption that you are comfortable with C# and the Unity platform. Instead, 
start with  the following video tutorials produced by Unity. They are quick and
simple tutorials and the narrator takes time to explain concepts.

1. [Roll a ball][RollABall-tut]
2. [Space Shooter][SpaceShooter-tut]

There are other [tutorials] which I skipped but still encourage you to try.
After I finished [Space Shooter][SpaceShooter-code] I was anxious to deep-dive
into Unity and which led me to Sebastian's video series. I will work through 
the other Unity tutorials—I'll commit my progress on GitHub along the way—after
I finish PillBlasta.

### Learn math and physics
The truth is you can create Unity applications without a strong understanding of
math and physics because, well, that's the point of Unity! It abstracts away
specialist knowledge so you can start to create right away. 

With that said, knowledge of both subjects will certainly aid your effectiveness
in the Unity platform. I'm currently working through pre-calculus text book.

[Seb]: https://github.com/SebLague
[CapsuleCollider]: https://docs.unity3d.com/Manual/class-CapsuleCollider.html
[RollABall-tut]: https://unity3d.com/learn/tutorials/projects/roll-ball-tutorial
[SpaceShooter-tut]: https://unity3d.com/learn/tutorials/projects/space-shooter-tutorial
[SpaceShooter-code]: https://github.com/jkereako/SpaceShooter
[tutorials]: https://unity3d.com/learn/tutorials
