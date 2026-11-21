import { defineConfig } from 'vitepress'

const repo = 'https://github.com/Devroots-Site/ShotPost'

// Bei Hosting unter einem Unterpfad (z.B. GitHub Pages) hier `base: '/ShotPost/'` setzen.
export default defineConfig({
  lang: 'de-DE',
  title: 'ShotPost',
  description: 'Entwickler-Dokumentation zu ShotPost',
  cleanUrls: true,
  // Zeitstempel je Seite aus dem letzten Git-Commit der Datei (siehe theme/LastUpdatedBadge.vue).
  lastUpdated: true,
  head: [['link', { rel: 'icon', type: 'image/svg+xml', href: '/logo.svg' }]],

  themeConfig: {
    logo: '/logo.svg',

    nav: [
      { text: 'Einstieg', link: '/einstieg/', activeMatch: '^/einstieg/' },
      { text: 'Architektur', link: '/architektur/', activeMatch: '^/architektur/' },
      { text: 'Framework', link: '/framework/', activeMatch: '^/framework/' },
      { text: 'Doku pflegen', link: '/doku/', activeMatch: '^/doku/' },
      {
        text: 'Offizielle Dokus',
        items: [
          { text: 'Avalonia UI', link: 'https://docs.avaloniaui.net/' },
          {
            text: 'CommunityToolkit.Mvvm',
            link: 'https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/',
          },
          { text: 'MySqlConnector', link: 'https://mysqlconnector.net/' },
          { text: 'CSharpier', link: 'https://csharpier.com/' },
          { text: 'VitePress', link: 'https://vitepress.dev/' },
        ],
      },
    ],

    sidebar: {
      '/einstieg/': [
        {
          text: 'Einstieg',
          items: [
            { text: 'Überblick', link: '/einstieg/' },
            { text: 'Schnellstart', link: '/einstieg/schnellstart' },
            { text: 'Projektstruktur', link: '/einstieg/projektstruktur' },
            { text: 'Entwicklungs-Workflow', link: '/einstieg/workflow' },
          ],
        },
      ],
      '/architektur/': [
        {
          text: 'Architektur',
          items: [
            { text: 'Überblick', link: '/architektur/' },
            { text: 'Oberfläche & Navigation', link: '/architektur/oberflaeche' },
            { text: 'Datenzugriff (SSMDB2)', link: '/architektur/datenzugriff' },
            { text: 'Modelle', link: '/architektur/modelle' },
            { text: 'Einstellungen & Theme', link: '/architektur/einstellungen' },
            { text: 'Payload', link: '/architektur/payload' },
          ],
        },
      ],
      '/framework/': [
        {
          text: 'Framework',
          items: [
            { text: 'Avalonia in Kürze', link: '/framework/' },
            { text: 'MVVM mit dem Toolkit', link: '/framework/mvvm' },
            { text: 'Rezept: neue Seite', link: '/framework/neue-seite' },
          ],
        },
      ],
      '/doku/': [
        {
          text: 'Doku pflegen',
          items: [{ text: 'VitePress & Automatik', link: '/doku/' }],
        },
      ],
    },

    socialLinks: [{ icon: 'github', link: repo }],

    search: {
      provider: 'local',
      options: {
        translations: {
          button: { buttonText: 'Suchen', buttonAriaLabel: 'Suchen' },
          modal: {
            displayDetails: 'Details anzeigen',
            resetButtonTitle: 'Suche zurücksetzen',
            backButtonTitle: 'Suche schließen',
            noResultsText: 'Keine Treffer für',
            footer: {
              selectText: 'auswählen',
              navigateText: 'navigieren',
              closeText: 'schließen',
            },
          },
        },
      },
    },

    lastUpdated: {
      text: 'Zuletzt aktualisiert',
      formatOptions: { dateStyle: 'medium', timeStyle: 'short' },
    },
    outline: { label: 'Auf dieser Seite', level: [2, 3] },
    docFooter: { prev: 'Vorherige Seite', next: 'Nächste Seite' },
    returnToTopLabel: 'Nach oben',
    sidebarMenuLabel: 'Menü',
    darkModeSwitchLabel: 'Darstellung',
    lightModeSwitchTitle: 'Zum hellen Design wechseln',
    darkModeSwitchTitle: 'Zum dunklen Design wechseln',
    footer: { message: 'ShotPost – Entwickler-Dokumentation' },
  },
})
