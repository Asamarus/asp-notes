import { Plugin, ButtonView, View, Dialog } from '../NoteEditor'

const tableIcon =
  '<svg viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path d="M8 2v5h4V2h1v5h5v1h-5v4h.021l-.172.351-1.916.28-.151.027c-.287.063-.54.182-.755.341L8 13v5H7v-5H2v-1h5V8H2V7h5V2h1zm4 6H8v4h4V8z" opacity=".6"></path><path d="m15.5 11.5 1.323 2.68 2.957.43-2.14 2.085.505 2.946L15.5 18.25l-2.645 1.39.505-2.945-2.14-2.086 2.957-.43L15.5 11.5zM17 1a2 2 0 0 1 2 2v9.475l-.85-.124-.857-1.736a2.048 2.048 0 0 0-.292-.44L17 3H3v14h7.808l.402.392L10.935 19H3a2 2 0 0 1-2-2V3a2 2 0 0 1 2-2h14z"></path></svg>'

export default class TabularData extends Plugin {
  get requires() {
    return [Dialog]
  }
  init() {
    const editor = this.editor

    editor.ui.componentFactory.add('tabularData', (locale) => {
      const view = new ButtonView(locale)

      view.set({
        label: 'Insert tabular data',
        icon: tableIcon,
        tooltip: true,
      })

      view.on('execute', () => {
        const dialog = this.editor.plugins.get('Dialog')

        const modalContent = new View(locale)

        modalContent.setTemplate({
          tag: 'div',
          attributes: {
            style: {
              padding: 'var(--ck-spacing-large)',
              whiteSpace: 'initial',
              width: '400px',
              maxWidth: '500px',
            },
            tabindex: -1,
          },
          children: [
            {
              tag: 'textarea',
              attributes: {
                class: 'ck-textarea',
                placeholder: 'Paste tabular data here...',
                style: {
                  width: '100%',
                  height: '200px',
                },
              },
            },
          ],
        })

        dialog.show({
          id: 'tabularData',
          title: 'Insert tabular data',
          content: modalContent,
          actionButtons: [
            {
              label: 'Add',
              class: 'ck-button-action',
              withText: true,
              onExecute: () => {
                const textarea = modalContent.element?.querySelector('textarea')
                const data = textarea?.value

                if (data) {
                  editor.model.change(() => {
                    const rows = data.split('\n').map((x) => x.split('\t'))

                    let tableHTML = '<figure class="table">\n<table>\n'

                    tableHTML += '  <thead>\n    <tr>'
                    rows[0].forEach((cell) => {
                      tableHTML += `\n      <th>${cell}</th>`
                    })
                    tableHTML += '\n    </tr>\n  </thead>\n  <tbody>'

                    for (let i = 1; i < rows.length; i++) {
                      tableHTML += '\n    <tr>'
                      rows[i].forEach((cell) => {
                        tableHTML += `\n      <td>${cell}</td>`
                      })
                      tableHTML += '\n    </tr>'
                    }

                    tableHTML += '\n  </tbody>\n</table>\n</figure>'

                    const viewFragment = editor.data.processor.toView(tableHTML)
                    const modelFragment = editor.data.toModel(viewFragment)

                    editor.model.insertContent(modelFragment, editor.model.document.selection)
                  })
                }

                dialog.hide()
              },
            },
          ],
        })
      })

      return view
    })
  }
}
