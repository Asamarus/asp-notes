const fs = require('fs')
const path = require('path')

const indexPath = path.join(__dirname, './dist/index.html')
const assetsPath = path.join(__dirname, './dist/assets')
const targetAssetsPath = path.join(__dirname, '../ProjectTemplate/wwwroot/assets')
const viewPath = path.join(__dirname, '../ProjectTemplate/Views/Home/Index.cshtml')

const data = fs.readFileSync(indexPath, 'utf8')

const jsFileName = data.match(/src="\/assets\/(.*?)"/)[1]
const cssFileName = data.match(/href="\/assets\/(.*?)"/)[1]
const jsHash = jsFileName.match(/-([^.]*).js/)[1]
const cssHash = cssFileName.match(/-([^.]*).css/)[1]

// Delete all files and folders in the target directory
fs.rmSync(targetAssetsPath, { recursive: true, force: true })

// Create the target directory
fs.mkdirSync(targetAssetsPath, { recursive: true })

// Copy all files from the assets directory to the target directory
fs.readdirSync(assetsPath).forEach((file) => {
  fs.copyFileSync(path.join(assetsPath, file), path.join(targetAssetsPath, file))
})

fs.readFile(viewPath, 'utf8', function (err, data) {
  if (err) {
    console.error(err)
    return
  }

  const newPathCss = `~/assets/index-${cssHash}.css`
  const newPathJs = `~/assets/index-${jsHash}.js`

  // Replace the href and src attributes
  const updatedData = data
    .replace(/(href=")([^"]*)(" data-id="main-css")/, `$1${newPathCss}$3`)
    .replace(/(src=")([^"]*)(" data-id="main-js")/, `$1${newPathJs}$3`)

  // Write the updated HTML back to the file
  fs.writeFile(viewPath, updatedData, function (err) {
    if (err) {
      console.error(err)
    } else {
      console.log('Deployment completed successfully!')
    }
  })
})
