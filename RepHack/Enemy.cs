namespace RepHack;
class Enemy : Entity
{
    public enum EnemyType { Goblin , Slime , Dragon};
    public EnemyType enemyType;

    public virtual void Act(TurnContext ctx)
    {
        (int x, int y) pos = ctx.pathfinding.GetNextStep(this, ctx.distanceMap, (x ,y) => ctx.IsOccupied(x, y));
            
        if(pos.x == ctx.player.X && pos.y == ctx.player.Y)
        {
            ctx.player.TakeDamage(this.Attack);
        }
        else
        {
            this.Move(pos.x - this.X, pos.y - this.Y);
        }
    }

    public virtual void OnHit(Player p, TurnContext ctx)
    {
        
    }
}

class Slime : Enemy
{
    public Slime()
    {
        MaxHp = 10;
        Attack = 3;
        Symbol = 'S';
    }
}

class Bat : Enemy
{
    public Bat()
    {
        MaxHp = 8;
        Attack = 4;
        Symbol = 'B';
    }
    public override void Act(TurnContext ctx)
    {
        //50퍼센트 확률로 랜덤방향 이동
    }
}

class Goblin : Enemy
{
    public Goblin()
    {
        MaxHp = 15;
        Attack = 5;
        Symbol = 'G';
    }
}

class Orc : Enemy
{
    public Orc()
    {
        MaxHp = 25;
        Attack = 8;
        Symbol = 'O';
    }
}

class Werewolf : Enemy
{
    public Werewolf()
    {
        MaxHp = 30;
        Attack = 10;
        Symbol = 'W';
    }
    public override void Act(TurnContext ctx)
    {
        //추격 + 근접할 시 공격력 증가
    }
}

class Vampire : Enemy
{
    public Vampire()
    {
        MaxHp = 35;
        Attack = 12;
        Symbol = 'V';
    }
    public override void Act(TurnContext ctx)
    {
        //추격 + 공격 성공시 Hp회복
    }
}

class DragonHatchling : Enemy
{
    public DragonHatchling()
    {
        MaxHp = 40;
        Attack = 10;
        Symbol = 'h';
    }
    public override void Act(TurnContext ctx)
    {
        //추격 + 원거리공격 예고
    }
}

class Dragon : Enemy
{
    public Dragon()
    {
        MaxHp = 60;
        Attack = 15;
        Symbol = 'D';
    }
    public override void Act(TurnContext ctx)
    {
        //추격 + 즉발 원거리 공격
    }
}