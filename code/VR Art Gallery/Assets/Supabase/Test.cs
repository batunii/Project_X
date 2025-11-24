using UnityEngine;
using Supabase;

public class SupabaseManager : MonoBehaviour
{
    private Client supabaseClient;

    async void Start()
    {
        supabaseClient = new Client("https://jdorkglqkatydqxcgshu.supabase.co", "sb_publishable__CS5YpEcdfuUKljKCVjfQw_dPKw-gW1");
        await supabaseClient.InitializeAsync();

        Debug.Log("Supabase initialized.");
    }
}
