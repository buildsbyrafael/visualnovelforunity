# Visual Novel para Unity

Este repositório contém um sistema de Visual Novel desenvolvido em Unity, projetado para criar experiências narrativas e interativas, com gerenciamento de cenas, transições visuais, transições sonoras e opções de escolhas.

### Estrutura e Componentes Principais

O sistema é modular e organizado em torno de componentes-chave, que trabalham juntos para gerenciar o fluxo da história.

- **StoryGamePlayer.cs:** Este é o ponto principal do jogo. Ele gerencia a sequência de capítulos, representados por objetos **StoryCap**, e controla o fluxo de transição entre eles. A reprodução de vídeos de carregamento também é orquestrada por este componente.
- **StoryCapPlayer.cs:** Este componente é central para a reprodução de um único capítulo. Ele é responsável por ler o arquivo de texto do capítulo, gerenciar as cenas, controlar a trilha sonora (música e efeitos sonoros) e orquestrar a exibição do diálogo.
- **BottomBarController.cs:** Gerencia a interface de usuário. Ele exibe o texto, processa comandos embutidos e lida com a lógica de avanço da fala, incluindo a opção de pular a frase atual.
- **CharacterLayerManager.cs:** Controla a exibição e a animação dos personagens. Ele gerencia diferentes "slots" para os personagens e lida com transições de aparecimento e desaparecimento (fade in/fade out).
- **ChoiceUIController.cs:** Este componente gerencia a interface e a lógica para exibir as opções de escolha. Ele interage com o **ChoiceManager** para registrar a decisão do jogador e continuar a narrativa com base na escolha feita.
- **ScreenFader.cs:** É responsável pelas transições de tela, como o fade-in e fade-out, que são utilizados para mudar o plano de fundo da cena de forma suave.
- **TextArchitect.cs:** É um utilitário que implementa diferentes métodos para a exibição do texto, como o "efeito de máquina de escrever" (typewriter) e o "efeito de fade" (fade).

### Técnicas de Programação e Padrões de Projeto

O sistema utiliza diversas técnicas de programação e padrões de design, comuns no desenvolvimento em Unity.

- **Coroutines:** São amplamente utilizadas para gerenciar ações assíncronas e sequenciais. Por exemplo, a reprodução de um capítulo inteiro (**PlayCapCoroutine**), a transição de tela (**FadeTransition**) e a construção do texto (**Building**) são controladas por coroutines. Isso evita o travamento do jogo durante operações demoradas.
- **ScriptableObjects:** São utilizados para criar ativos de dados reutilizáveis, como **StoryCap** e **Speaker**. Isso permite que os dados do jogo, como capítulos, músicas e informações dos personagens sejam configurados diretamente no editor, separando a lógica do código dos dados.
- **Padrão Singleton:** Implementado nos gerenciadores **SpeakerDatabase** e **ChoiceManager**. Esse padrão garante que haja apenas uma instância desses componentes, facilitando o acesso global a dados de personagens e escolhas.
- **Expressões Regulares (Regex):** São empregadas na classe **StoryTextParser** para analisar o arquivo de texto da história e extrair informações como IDs de cenas, planos de fundo e condições. A classe **DL_DIALOGUE_DATA** também as utiliza para processar comandos complexos embutidos no texto, como efeitos sonoros ou alterações de sprite.
- **Modularidade e Componentização:** O design do sistema é baseado na filosofia de componentização da Unity. Cada script tem uma responsabilidade clara, como gerenciar o diálogo (**BottomBarController**), controlar a música (**StoryCapPlayer**) ou as transições de tela (**ScreenFader**), tornando o sistema fácil de manter e estender.

### Tecnologias e Linguagens 

[![Unity](https://img.shields.io/badge/Unity-20232A?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/pt)
[![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91?style=for-the-badge&logo=visual-studio&logoColor=white)](https://visualstudio.microsoft.com/pt-br/)
[![C%23](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/pt-br/dotnet/csharp/)
