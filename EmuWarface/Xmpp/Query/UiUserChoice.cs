namespace EmuWarface.Xmpp.Query
{
	public static class UiUserChoice
	{
		/*
         * <ui_user_choice>
<choice choice_from="lobby_pvp_game_room" choice_id="join_quickplay_session" choice_result="1" />
</ui_user_choice>
         */

		[Query(IqType.Get, "ui_user_choice")]
		public static void UiUserChoiceSerializer(Client client, Iq iq)
		{
			//TODO
		}
	}
}
