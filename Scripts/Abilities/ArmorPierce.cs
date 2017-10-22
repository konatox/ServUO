using System;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// Strike your opponent with great force, partially bypassing their armor and inflicting greater damage. Requires either Bushido or Ninjitsu skill
    /// </summary>
    public class ArmorPierce : WeaponAbility
    {
        public static Dictionary<Mobile, Timer> _Table = new Dictionary<Mobile, Timer>();

        public ArmorPierce()
        {
        }

        public override SkillName GetSecondarySkill(Mobile from)
        {
            return from.Skills[SkillName.Ninjitsu].Base > from.Skills[SkillName.Bushido].Base ? SkillName.Ninjitsu : SkillName.Bushido;
        }

        public override int BaseMana
        {
            get
            {
                return 30;
            }
        }

        public override double DamageScalar
        {
            get
            {
                return Core.HS ? 1.0 : 1.5;
            }
        }

        public override bool RequiresSE
        {
            get
            {
                return true;
            }
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063350); // You pierce your opponent's armor!
            defender.SendLocalizedMessage(1063351); // Your attacker pierced your armor!            

            if (Core.HS)
            {
                if (_Table.ContainsKey(defender))
                {
                    if (attacker.Weapon is BaseRanged)
                        return;

                    _Table[defender].Stop();
                }

                _Table[defender] = Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(3), RemoveEffects, defender);
            }

            defender.FixedParticles(0x3728, 1, 26, 0x26D6, 0, 0, EffectLayer.Waist);
        }

        public static void RemoveEffects(Mobile m)
        {
            if (IsUnderEffects(m))
            {
                _Table.Remove(m);
            }
        }

        public static bool IsUnderEffects(Mobile m)
        {
            return _Table.ContainsKey(m);
        }
    }
}