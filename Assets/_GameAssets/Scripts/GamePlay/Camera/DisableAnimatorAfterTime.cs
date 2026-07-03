using UnityEngine;

// Script'in adı DisableAnimatorAfterTime kalabilir, sorun değil.
public class DisableAnimatorAfterTime : MonoBehaviour 
{
    // Start() ve Invoke() metodlarını sildik.

    // Bu, animasyon olayı tarafından çağrılacak yeni fonksiyonumuz.
    // public olması ŞART.
    public void DisableMyAnimator() 
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Animator'ü kapat.
            // Bu, kamerayı en son bıraktığı pozisyonda kilitler.
            animator.enabled = false;
        }
    }
}