# @yafnet/theme-kit

Create and build custom themes for [YetAnotherForum.NET](https://www.yetanotherforum.net/) without the forum source code.

The kit contains the SASS sources of all themes included in YAF.NET (the `yaf` theme and the [Bootswatch](https://bootswatch.com) themes), the forum styles the themes need, and a small command line tool to create and build themes.

> **Use the kit version of your forum version**: the kit 4.0.11 builds themes for YAF.NET 4.0.11. The themes contain styles for the forum markup and Bootstrap version of a release.

## Requirements

* [Node.js](https://nodejs.org/) 20 or newer
* Optional, for the email templates in the colors of your theme: the [.NET SDK](https://dotnet.microsoft.com/download) and the `bootstrap-email` tool:

  ```sh
  dotnet tool install --global BootstrapEmail.Net.Cli
  ```

  Without it the prebuilt email templates of the theme you started from are used.

## Quick start

```sh
mkdir my-forum-theme
cd my-forum-theme
npm init -y
npm install --save-dev @yafnet/theme-kit@4.0.11

npx yafnet-theme new mytheme --from cosmo
# change themes/mytheme/_variables.scss
npx yafnet-theme build
```

Copy the folder `dist/mytheme` to the `wwwroot/css/themes` folder of your forum and select the theme in **Admin > Board Settings > Theme**.

## Commands

| Command | Description |
|---------|-------------|
| `yafnet-theme list` | Lists the themes of the kit you can start from. |
| `yafnet-theme new <name> [--from <theme>]` | Creates the theme `<name>` in the themes folder as a copy of a theme of the kit (default: `yaf`, plain Bootstrap). |
| `yafnet-theme build [<name>...]` | Builds all themes in the themes folder, or the named ones, into the output folder. |
| `yafnet-theme watch [<name>...]` | Builds the CSS again whenever a file in the themes folder changes. |

| Option | Description |
|--------|-------------|
| `--themes <folder>` | Folder of the theme sources (default: `themes`). |
| `--out <folder>` | Output folder (default: `dist`). Use the `wwwroot/css/themes` folder of a local forum to see the changes right away. |
| `--no-email` | `build`: only the CSS, not the email templates. |

## A theme

```
themes/mytheme/
├── _variables.scss               Bootstrap variables of the website: colors, fonts, spacing...
├── _bootswatch.scss              extra styles of a Bootswatch theme
├── bootstrap-forum.scss          main file: variables, Bootstrap, forum styles
├── _variables-email.scss         variables of the email templates (yaf theme only)
├── bootstrap-email.scss          main file of the email styles
├── bootstrap_email*.json         bootstrap-email configs (for the YAF.NET source build)
└── yafnet-theme.json             the theme of the kit it was created from
```

The build creates:

```
dist/mytheme/
├── bootstrap-forum.min.css
├── EmailTemplate.html
└── DigestTopicTemplate.html
```

All three files are needed by the forum.

`bootstrap-forum.scss` imports Bootstrap and the forum styles from the kit:

```scss
@import "_variables.scss";
@import "bootstrap/scss/bootstrap";

@import "forum/chat.scss";
@import "choices/bs-theme.scss";
@import "netStack.scss";
```

Keep these imports. Your own CSS rules go at the end of the file.

All Bootstrap variables are listed in `node_modules/bootstrap/scss/_variables.scss` (dark mode: `_variables-dark.scss`).

## Update to a new YAF.NET version

Install the kit of the new version and build your theme again:

```sh
npm install --save-dev @yafnet/theme-kit@<new version>
npx yafnet-theme build
```

## More

See [Create a custom theme](https://github.com/YAFNET/YAFNET/wiki/Customization) in the YAF.NET wiki.

## License

Apache-2.0. The Bootswatch themes are licensed under the MIT license (see `LICENSE.md` in the theme folders).
